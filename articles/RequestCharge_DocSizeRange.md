# Request Charge vs. document size observations

Common scenario where a document can grow significantly by storing a large json blurb in one property, some "property headers" (probably extracted from received json) and index policy optimized for the properties that will be queried.

Sample document with a faker library generating RawJSON.

```
{
    "id": "ca43bff8-a550-4a3a-94ed-cc0fd87503d0",
    "PartitionKey": "96",
    "Name": "0cbf4470-d4d2-4083-a5a7-e75d27037169",
    "Description": "0cbf4470-d4d2-4083-a5a7-e75d27037169",
    "BusinessTimestamp": "2023-04-27T12:05:16.733192Z",
    "State": "Hot",
    "ArchiveUri": null,
    "RawJSON": "{\"properties\":{\"human-resource\":\"Money Market Account\",\"Tasty\":\"engage Rubber\",\"niches\":\"program Concrete\",\"indexing\":\"Mall Markets\",\"initiatives\":\"Generic Fresh Tuna\",\"Chief\":\"New Jersey\",\"Crossing\":\"Orchestrator\",.........",
}
```
<br/>
<br/>

## Create request units, minimal indexing (only on PartitionKey)

container.CreateItemAsync<type>(e, new PartitionKey(e.PartitionKey));

Request body size mapping to RUs
- 769 bytes: 5.9 RUs
- 1156 ~ 1557 bytes: 7.24 RUs
- 1905 ~ 3895 bytes: 7.62 RUs
- 4234 ~ 7814 bytes: 8 RUs
- 8115 ~ 15900 bytes: 10.48 RUs
- 16232 ~ 32388 bytes: 12.19 RUs
- 32623 ~ 65322 bytes: 23.62 RUs
- 65519 ~ 130877 bytes: 49.14 RUs
- 130921 ~ 261913 bytes: 99.24 RUs
- 262006 ~ 319563 bytes: 185.52 RUs

Obs: As additional properties are added, the effective document size is different form the request body size. Max doc size in this test had 319563 bytes, but last range should be wider that that (512 KB). 

<br/>Very intersting and informative. Request charge is not linear with document size increase, but work in ranges.
<br/> Where have I seen similar ranges before...? Ah! (2^10), (2^11), (2^12), (2^13), (2^14)

[Companion file](../Presentations/ReferenceFiles/CreateRUs_MinimalIndexing.xlsx)
