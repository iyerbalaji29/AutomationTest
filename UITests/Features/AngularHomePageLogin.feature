Feature: Angular Home Page Login
    As a user
    I want to verify that the Angular home page loads successfully
    And the login button is visible
    So that I can access the application

@smoke @homepage
Scenario: Verify Angular home page loads successfully and login button is visible
    Given I navigate to the Angular home page
    When the Angular page finishes loading
    Then the Angular page should be loaded successfully
    And the Angular logo should be displayed
    And the login button should be visible on the page

@smoke @homepage
Scenario: Verify all key elements on Angular home page
    Given I navigate to the Angular home page
    When the Angular page finishes loading
    Then the Angular page should be loaded successfully
    And the Angular logo should be displayed
    And the page title should contain "Angular"
    And the login button should be visible on the page
    And the Get Started button should be displayed
    And the Docs link should be visible

@chrome @homepage
Scenario: Verify login button functionality
    Given I navigate to the Angular home page
    When the Angular page finishes loading
    Then the login button should be visible on the page
    And I can interact with the login button
