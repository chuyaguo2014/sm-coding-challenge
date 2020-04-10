# BR StatMilk Coding Exercise

This is a small [dotnet core](https://www.microsoft.com/net) project that presents a few challenges to the user.  Instructions for completion are as followed.  To get the project running, execute the following:

      git clone git@github.com:br/sm-coding-challenge.git
      cd sm-coding-challenge
      dotnet restore
      dotnet run

## Problem

There are various third-party APIs that we ingest data from, including Turner/Sports Data, Twitter and Instagram. Our systems then store this data and return it for future date.  Our services must be fast and reliable given an influx in service downtime/degredation from providers as well as unexpected traffic spikes from our users.

One of the endpoints we ingest data from is similiar to this gist: https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/basic.json. This json is a subset of one week's box score data for NFL.

Currently this project has an 'IDataProvider' interface that has a a single method to get a player by a source Player ID.  This method fetches the above data and returns the first instance of a player if found.  Please update the project to add/meet the list of items in the requirements section.

## Requirements

* All urls on the home page should return without error
  * Done except the _LatestPlayer_ one
* Update the DataProvider interface and implementation to use async/await
  * Done
* Refactor the DataProvider implementation as you see fit.  Comment on the changes you made.
  * Done
* Add missing player attributes to the fetch so all data from the data provider is returned to the front-end
  * Done
* Duplicates should be removed from the existing GetPlayers result(s)
  * Done
* Implement the "LatestPlayers" method to return a structure similar to: https://gist.githubusercontent.com/RichardD012/a81e0d1730555bc0d8856d1be980c803/raw/3fe73fafadf7e5b699f056e55396282ff45a124b/output.json
  * **QUESTION** - what does _LatestPlayer_ mean?
* All responses should be performant.  None of these should take longer than a few miliseconds.  There are multiple solutions/avenues to this but think about the frequency that you fetch the data and different ways to mitigate iterating over too much data or making multiple requests.
  * Done. The _Long_List_Of_Players_ method takes about 75ms on average on my 2015 macbook pro. 
* If you remove/change/invalidate the url from the DataProvider fetch method, the system should still "work" (fail gracefully - up to you on your definition of work).
  * Done.

### Considerations in Design

* What would happen if the remote endpoint goes down?  What are some ways to mitigate this that are transparent to the user?
  * The app would break/throw errors as of right now but we can consider things like: 
    * caching the external api response (in memory or in redis depending on the size of the response)
    * have our app fetch from cache first and 
* What are some ways to prevent multiple calls to the data endpoint?
  * again, we can cache the response (for example, at app startup, call the external api once) 
  * or save to a DB/S3 bucket and have another background/scheduled task/lambda that calls the external api every few hours/days to update the data if necessary. But our app will talk to the DB/S3 only. 
* This data set is not updated very frequently (once a week), what are some ways we could take advantage of this in our system?
  * We can have our own scheduled tasks to call the api; 
  * or, if the external api has webhooks/events we can subscribe to, then we can write a aws lambda function that triggers when the data in the external is updated, and then we will go fetch it (so not on a schedule, just only when necessary)

## Notes

There is no limit to what is acceptable as far as libraries, changes, and methodologies goes.  Feel free to add/remove/change methods, abstractions, etc. to facilitate your needs.  Feel free to comment on areas that are dubious or may present challenges in a real-world environment.