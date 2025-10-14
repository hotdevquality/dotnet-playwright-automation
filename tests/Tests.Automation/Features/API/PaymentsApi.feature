Feature: Payments API integration

    @api @http
    Scenario: Submit bank payment via HTTP POST
        Given a bank payment "Alice" "10-11-12" "12345678" 100.50 "GBP"
        When I submit the payment via API
        Then the API response status should be "accepted"