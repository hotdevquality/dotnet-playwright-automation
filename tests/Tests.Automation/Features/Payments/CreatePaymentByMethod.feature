Feature: Create payment by method
As a user
I want to create payments using different methods
So that country-specific methods are covered

    @ui @payment @cheque
    Scenario: Create UK cheque payment
        Given I log in as "initiator"
        And I open the Create Payment page
        And I choose payment method "cheque"
        And I enter cheque number "CHQ-001234"
        When I submit the payment
        Then I should be on the payments list

    @ui @payment @card
    Scenario: Create UK credit card payment
        Given I log in as "initiator"
        And I open the Create Payment page
        And I choose payment method "credit card"
        And I enter card number "4111111111111111" and expiry "12/29"
        When I submit the payment
        Then I should be on the payments list

    @ui @payment @paypal
    Scenario: Create EU PayPal payment
        Given I log in as "initiator"
        And I open the Create Payment page
        And I choose payment method "paypal"
        And I enter PayPal email "payer@example.com"
        When I submit the payment
        Then I should be on the payments list