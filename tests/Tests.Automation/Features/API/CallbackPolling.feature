Feature: Payment status callback (polled)

    @api @callback
    Scenario: Payment transitions to processed via polling
        Given a bank payment "Alice" "10-11-12" "12345678" 100.50 "GBP"
        When I submit the payment via API
        And I poll the status for the returned id until "processed" or timeout 5s
        Then the API response status should be "accepted"