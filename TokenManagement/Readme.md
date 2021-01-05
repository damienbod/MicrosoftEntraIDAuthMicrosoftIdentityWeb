# Using GRAPH REST API

POST https://graph.microsoft.com/v1.0/policies/tokenLifetimePolicies

{
  "definition": [
    "{\"TokenLifetimePolicy\":{\"Version\":1,\"AccessTokenLifetime\":\"01:30:00\"}}"
  ],
  "displayName": "xxxxxx",
  "isOrganizationDefault": false
}

## Links

https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-configurable-token-lifetimes#configurable-token-lifetime-properties

https://stackoverflow.com/questions/65278010/how-to-set-the-access-token-lifetime-for-an-app-using-the-microsoft-graph-api

https://docs.microsoft.com/en-us/graph/api/tokenlifetimepolicy-post-tokenlifetimepolicies?view=graph-rest-1.0&tabs=http

https://docs.microsoft.com/en-us/graph/api/serviceprincipal-post-claimsmappingpolicies?view=graph-rest-1.0&tabs=http
