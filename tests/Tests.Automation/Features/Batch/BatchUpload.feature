Feature: Batch upload via SFTP
    @batch @sftp @csv
    Scenario: Upload CSV batch
        Given a batch containing the following payments
          | Country | Method | Recipient | SortCode | AccountNumber | Iban                   | Amount | Currency |
          | UK      | bank   | Alice     | 10-11-12 | 12345678      |                        | 100.5  | GBP      |
          | EU      | bank   | Bob       |          |               | DE12345678901234567890 | 55.75  | EUR      |
        When I build a CSV batch file
        And I upload the batch to SFTP
        Then the file should exist in the SFTP inbound folder

    @batch @sftp @excel
    Scenario: Upload Excel batch
        Given a batch containing the following payments
          | Country | Method | Recipient | SortCode | AccountNumber | Iban                   | Amount | Currency |
          | UK      | bank   | Alice     | 10-11-12 | 12345678      |                        | 100.5  | GBP      |
          | EU      | bank   | Bob       |          |               | DE12345678901234567890 | 55.75  | EUR      |
        When I build an Excel batch file
        And I upload the batch to SFTP
        Then the file should exist in the SFTP inbound folder
        
    @batch @sftp @csv @methods
    Scenario Outline: Build CSV batch for different methods
        Given a batch containing the following payments
          | Country   | Method   | Recipient | SortCode | AccountNumber | Iban                   | Amount | Currency   |
          | <Country> | <Method> | Alice     | 10-11-12 | 12345678      | DE12345678901234567890 | 42.00  | <Currency> |
        When I build a CSV batch file
        And I upload the batch to SFTP
        Then the file should exist in the SFTP inbound folder

        Examples:
          | Country | Method      | Currency |
          | UK      | cheque      | GBP      |
          | UK      | credit card | GBP      |
          | EU      | paypal      | EUR      |
        