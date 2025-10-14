Feature: UI validation error on create payment

    @ui @negative
    Scenario: Invalid bank account shows error
        Given I log in as "initiator"
        And I open the Create Payment page
        And I choose payment method "bank"
        And I enter bank account number "BADACCT"
        When I submit the payment
        Then I should see a validation error "Invalid account number"