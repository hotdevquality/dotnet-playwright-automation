Feature: Payments API negative cases

    @api @http @negative
    Scenario: Invalid bank details return 422
        Given a bank payment "Alice" "10-11-12" "BADACCT" 0 "GBP"
        When I submit the payment via API with case "invalid"
        Then the API should return status code 422 and error "invalid_payment_details"

    @api @http @negative
    Scenario: Unsupported method returns 400
        Given a payment request with method "crypto"
        When I submit the payment via API with case "unsupported"
        Then the API should return status code 400 and error "unsupported_method"

    @api @http @negative
    Scenario: Bank outage returns 503
        Given a bank payment "Bob" "10-11-12" "12345678" 10.50 "GBP"
        When I submit the payment via API with case "fail"
        Then the API should return status code 503 and error "bank_unavailable"