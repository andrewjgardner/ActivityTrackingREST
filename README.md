#Create an application in .NET (using C#) for bulding out some REST API
endpoints, as detailed below.

i) A client wants to be able to record the time they spend on different
types of activities as part of them providing a legal service. Create an
endpoint which allows the client to save an activity with the following
information:
- Id
- FirmId
- Name
- Type
- Date Time Started
- Date Time Finished
- Elapsed Time

'Type' can be Phone Call, Email, Document, Appointment.

* For this test, your backing data store can be in memory, text file, 
embedded sql-lite, ... whatever you deem is easiest to prove your point.

ii) After creating an activity, a client wants to be able to edit the activity.
Create an endpoint which allows the client to edit any of the
following properties for an activity:
- Name
- Date Time Started
- Date Time Finished
- Elapsed Time

iii) A client wants to be able to view the total time spent on each activity
Type for a specified date range. Create an endpoint which groups activities
by Type and returns a collection of items with this structure:
- Type
- Total Elapsed Time
- Activities (the activities which are grouped within this record)

iv) For activities with the 'Email' Type, we would like to store an additional
property, an array of 'Attachments':
Attachments: [
		{ id: '...', name: '...' },
		{ id: '...', name: '...' },
		....
	     ]

Feel free to populate the above item values with whatever you deem fit.

When returning activity data from endpoint iii), if the activity Type is 'Email',
we wish to see the 'Attachments' field for each of the returned activities.

If the Type isn't 'Email', the attachments property shouldn't be there.

--------
You can use postman to create and test cURLs.

*** if you have time; write tests for logic where necessary.
--------

# How to use

i) 
curl -X 'POST' \
  'https://localhost:7003/api/v1/activity' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json-patch+json' \
  -d '{
  "name": "string",
  "dateTimeStarted": "2022-06-19T22:06:58.747Z",
  "dateTimeFinished": "2022-06-19T22:06:58.747Z",
  "elapsedTime": "00:30:00",
  "id": "testtesttest",
  "firmId": "string",
  "type": 0,
  "attachments": [
    {
      "id": "testtesttesttest",
      "name": "string"
    }
  ]
}'

ii)
curl -X 'PATCH' \
  'https://localhost:7003/api/v1/activity?id=TestThree' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json-patch+json' \
  -d '[
  {
    "path": "/elapsedTime",
    "op": "add",
    "value": "01:30:00"
  }
]'

iii)
curl -X 'GET' \
  'https://localhost:7003/api/v1/activity/types/2022-06-10T21%3A59%3A01.581Z/2022-06-30T21%3A59%3A01.581Z' \
  -H 'accept: text/plain'
