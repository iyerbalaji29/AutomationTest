Feature: Angular Home Page Tests
    As a tester
    I want to validate the Angular home page
    So that I can ensure all UI elements are working correctly

Background:
    Given I navigate to the Angular home page

@chrome @smoke @critical
Scenario: Verify Angular home page loads successfully
    Then the Angular page should be loaded successfully
    And the Angular logo should be displayed
    And the page title should contain "Angular"

@chrome @smoke @high
Scenario: Verify main navigation elements are visible
    Then the Angular logo should be displayed
    And the Get Started button should be displayed
    And the Docs link should be visible

@firefox @regression @medium
Scenario: Verify page navigation functionality
    When I click on the Docs link
    Then the Angular page should be loaded successfully
    And the page URL should contain "docs"

@edge @regression @medium
Scenario: Verify Angular page in different browser
    Then the Angular page should be loaded successfully
    And the Angular logo should be displayed

@chrome @screenshot @low
Scenario: Take screenshot of home page
    Then the Angular page should be loaded successfully
    And I take a screenshot of the home page

@chrome @smoke @regression @critical
Scenario: Verify page title and URL are correct
    Then the page title should contain "Angular"
    And the page URL should contain "angular.io"

@firefox @smoke @critical
Scenario: Verify Angular logo is clickable in Firefox
    Then the Angular logo should be displayed
    When I click on the Docs link
    Then the Angular page should be loaded successfully
