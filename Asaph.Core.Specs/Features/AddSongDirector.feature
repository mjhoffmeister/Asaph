Feature: AddSongDirector

Scenario: Someone tries to add a song director
	Given the person trying to add a song director is a <adderRank>
	And the new song director full name is <fullName>
	And the new song director email address is <emailAddress>
	And the new song director phone number is <phoneNumber>
	And the new song director rank is <rankName>
	When the song director add is handled
	Then the result message should be <message>

	Examples:
		| adderRank   | fullName | emailAddress       | phoneNumber  | rankName    | message                                          |
		| Grandmaster | John Doe | john.doe@gmail.com | 123-456-1234 | <null>      | John Doe was added.                              |
		| Grandmaster | Jane Doe | jane.doe@gmail.com | 987-654-9876 | Apprentice  | Jane Doe was added.                              |
		| Grandmaster | Jane Doe | jane.doe@gmail.com | 987-654-9876 | Journeyer   | Jane Doe was added.                              |
		| Grandmaster | John Doe | john.doe@gmail.com | 123-456-1234 | Master      | John Doe was added.                              |
		| Grandmaster | Jane Doe | jane.doe@gmail.com | 987-654-9876 | Grandmaster | Jane Doe was added.                              |
		| Grandmaster | John Doe | john.doe@gmail.com | <null>       | <null>      | John Doe was added.                              |
		| Grandmaster | Jane Doe | <null>             | 987-654-9876 | <null>      | Email address is required.                       |
		| Grandmaster | <null>   | john.doe@gmail.com | 123-456-1234 | <null>      | Full name is required.                           |
		| Grandmaster | John Doe | 1234567890         | 123-456-1234 | <null>      | Invalid email address.                           |
		| Grandmaster | Jane Doe | jane.doe@gmail.com | 987-654-987  | <null>      | Invalid phone number.                            |
		| Grandmaster | John Doe | john.doe@gmail.com | 123-456-1234 | Associate   | Invalid rank.                                    |
		| Apprentice  | Jane Doe | jane.doe@gmail.com | 987-654-9876 | Apprentice  | You don't have permission to add song directors. |
		| Journeyer   | Jane Doe | jane.doe@gmail.com | 987-654-9876 | Apprentice  | You don't have permission to add song directors. |
		| Master      | Jane Doe | jane.doe@gmail.com | 987-654-9876 | Apprentice  | You don't have permission to add song directors. |


