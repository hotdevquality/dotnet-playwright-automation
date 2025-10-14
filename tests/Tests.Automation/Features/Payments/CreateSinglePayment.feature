Feature: Create single payment (UI)
  As an initiator
  I want to create a valid UK bank payment
  So that it moves to Pending Approval

  @ui @smoke @approval
  Scenario: Create valid UK bank payment
    Given I log in as "initiator"
    And I open the Create Payment page
    When I submit a UK bank payment with sort code "10-11-12" and account "12345678"
    Then the payment appears with status "Pending Approval"
