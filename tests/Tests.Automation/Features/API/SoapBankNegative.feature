Feature: SOAP Bank negative

    @api @soap @negative
    Scenario: SOAP fault returned by bank
        When I request bank status for account "FAULT" by SOAP
        Then the SOAP request should fail with a server error