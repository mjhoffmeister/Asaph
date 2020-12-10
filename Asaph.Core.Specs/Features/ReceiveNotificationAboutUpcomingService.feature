Feature: RecieveNotificationAboutUpcomingService
	In order to be reminded about a song service I'm scheduled to direct
	As a song director
	I want to recieve a notification 

@mytag
Scenario: Add two numbers
	Given the day is Monday
	When a song director is scheduled to lead singing the following Sunday
	Then the song director should receive a reminder