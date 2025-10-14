Feature: SOAP Bank integration

    @api @soap
    Scenario: Get bank status via SOAP
        When I request bank status for account "12345678" by SOAP
        Then the SOAP status should be "OK"