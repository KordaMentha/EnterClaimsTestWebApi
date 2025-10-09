# EnterClaimsTestWebApi help documentation

The API's are hosted on Azure - https://enterclaimtestwebapi-dtfvaqfzfqe5dgc5.australiasoutheast-01.azurewebsites.net

Below is a list of API's (Json)
{
  "openapi": "3.0.1",
  "info": {
    "title": "EnterClaimsTestWebApi",
    "version": "1.0"
  },
  "paths": {
    "/api/EnterClaimsTest/GetCoreDataForUserAllFields": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "parameters": [
          {
            "name": "empID",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "DOB",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "last4digitsofTFN",
            "in": "query",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/EnterClaimsTest/GetClaimIDForUser": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "parameters": [
          {
            "name": "empID",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "DOB",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "last4digitsofTFN",
            "in": "query",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/EnterClaimsTest/UpdateClaimForm": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "parameters": [
          {
            "name": "empID",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "DOB",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "last4digitsofTFN",
            "in": "query",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/EnterClaimsTest/GetCoreDataVerifyUser": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "parameters": [
          {
            "name": "empID",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "DOB",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "last4digitsofTFN",
            "in": "query",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/EnterClaimsTest/CheckForDuplicateClaim": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "parameters": [
          {
            "name": "empID",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "DOB",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "last4digitsofTFN",
            "in": "query",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/EnterClaimsTest/DeleteDuplicateClaim": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "parameters": [
          {
            "name": "claimID",
            "in": "query",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/EnterClaimsTest/GetBSBs": {
      "get": {
        "tags": [
          "EnterClaimsTest"
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Test/SimpleTest": {
      "get": {
        "tags": [
          "Test"
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    }
  },
  "components": { }
}
