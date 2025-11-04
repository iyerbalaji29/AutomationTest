#!/bin/bash

# Bash script to run automation tests locally with tag filtering
# Usage: ./RunTests.sh --tags "smoke,chrome" --browser "chrome" --environment "Test"

# Default values
TAGS=""
BROWSER="chrome"
ENVIRONMENT="Local"
CONFIGURATION="Debug"
OPEN_REPORT=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --tags)
            TAGS="$2"
            shift 2
            ;;
        --browser)
            BROWSER="$2"
            shift 2
            ;;
        --environment)
            ENVIRONMENT="$2"
            shift 2
            ;;
        --configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --open-report)
            OPEN_REPORT=true
            shift
            ;;
        --help)
            echo "Usage: ./RunTests.sh [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --tags <tags>              Comma-separated test tags (e.g., 'smoke,chrome')"
            echo "  --browser <browser>        Browser to use: chrome, firefox, edge, all (default: chrome)"
            echo "  --environment <env>        Environment: Test, Staging, Production, Local (default: Local)"
            echo "  --configuration <config>   Build configuration: Debug, Release (default: Debug)"
            echo "  --open-report              Open HTML report after execution"
            echo "  --help                     Display this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Display banner
echo "================================================"
echo "  Automation Test Framework - Test Runner     "
echo "================================================"
echo ""

# Display configuration
echo "Configuration:"
echo "  Tags: ${TAGS:-All}"
echo "  Browser: $BROWSER"
echo "  Environment: $ENVIRONMENT"
echo "  Configuration: $CONFIGURATION"
echo ""

# Set environment variable
export ENVIRONMENT=$ENVIRONMENT

# Clean previous results
echo "Cleaning previous test results..."
rm -rf UITests/TestResults 2>/dev/null
rm -rf UITests/TestReports 2>/dev/null
echo "Cleanup complete."
echo ""

# Build solution
echo "Building solution..."
dotnet build --configuration $CONFIGURATION --verbosity quiet

if [ $? -ne 0 ]; then
    echo "Build failed! Please fix build errors and try again."
    exit 1
fi

echo "Build successful."
echo ""

# Prepare test filter
FILTER=""
if [ -n "$TAGS" ]; then
    IFS=',' read -ra TAG_ARRAY <<< "$TAGS"
    FILTER_PARTS=()

    for tag in "${TAG_ARRAY[@]}"; do
        tag=$(echo "$tag" | xargs)  # Trim whitespace
        FILTER_PARTS+=("Category=$tag")
    done

    FILTER=$(IFS='&'; echo "${FILTER_PARTS[*]}")
fi

# Add browser filter if not "all"
if [ "$BROWSER" != "all" ]; then
    if [ -n "$FILTER" ]; then
        FILTER="${FILTER}&Category=${BROWSER}"
    else
        FILTER="Category=${BROWSER}"
    fi
fi

# Run tests
echo "Running tests..."
echo "Filter: $FILTER"
echo ""

TEST_ARGS="test UITests/UITests.csproj --configuration $CONFIGURATION --no-build --logger trx --logger console;verbosity=detailed --results-directory UITests/TestResults"

if [ -n "$FILTER" ]; then
    TEST_ARGS="$TEST_ARGS --filter \"$FILTER\""
fi

eval "dotnet $TEST_ARGS"
TEST_EXIT_CODE=$?

echo ""
echo "================================================"

# Parse test results
TRX_FILE=$(find UITests/TestResults -name "*.trx" -type f | head -n 1)

if [ -f "$TRX_FILE" ]; then
    # Extract test results using grep and sed
    TOTAL=$(grep -oP 'total="\K[^"]+' "$TRX_FILE" | head -n 1)
    PASSED=$(grep -oP 'passed="\K[^"]+' "$TRX_FILE" | head -n 1)
    FAILED=$(grep -oP 'failed="\K[^"]+' "$TRX_FILE" | head -n 1)
    SKIPPED=$(grep -oP 'notExecuted="\K[^"]+' "$TRX_FILE" | head -n 1)

    SUCCESS_RATE=0
    if [ "$TOTAL" -gt 0 ]; then
        SUCCESS_RATE=$(awk "BEGIN {printf \"%.2f\", ($PASSED / $TOTAL) * 100}")
    fi

    echo ""
    echo "Test Execution Summary:"
    echo "  Total Tests: $TOTAL"
    echo "  Passed: $PASSED"
    echo "  Failed: $FAILED"
    echo "  Skipped: $SKIPPED"
    echo "  Success Rate: ${SUCCESS_RATE}%"
    echo ""

    # Generate summary file
    SUMMARY_PATH="UITests/TestResults/summary.txt"
    cat > "$SUMMARY_PATH" << EOF
Test Execution Summary
=====================

Execution Time: $(date "+%Y-%m-%d %H:%M:%S")
Environment: $ENVIRONMENT
Browser: $BROWSER
Tags: ${TAGS:-All}

Results:
--------
Total Tests: $TOTAL
Passed: $PASSED
Failed: $FAILED
Skipped: $SKIPPED
Success Rate: ${SUCCESS_RATE}%

EOF

    echo "Summary saved to: $SUMMARY_PATH"
fi

# Check for HTML reports
HTML_REPORT=$(find UITests/TestReports -name "*.html" -type f | head -n 1)

if [ -f "$HTML_REPORT" ] && [ "$OPEN_REPORT" = true ]; then
    echo ""
    echo "Opening HTML report..."
    if command -v xdg-open &> /dev/null; then
        xdg-open "$HTML_REPORT"
    elif command -v open &> /dev/null; then
        open "$HTML_REPORT"
    else
        echo "Could not open report automatically. Please open manually: $HTML_REPORT"
    fi
fi

# Display artifact locations
echo ""
echo "Test Artifacts:"
echo "  Test Results: $(pwd)/UITests/TestResults"
echo "  HTML Reports: $(pwd)/UITests/TestReports"
echo "  Screenshots: $(pwd)/UITests/Screenshots"
echo "  Logs: $(pwd)/UITests/Logs"

echo ""
echo "================================================"

# Exit with appropriate code
if [ $TEST_EXIT_CODE -ne 0 ] || [ "$FAILED" -gt 0 ]; then
    echo "Tests FAILED!"
    exit 1
else
    echo "All tests PASSED!"
    exit 0
fi
