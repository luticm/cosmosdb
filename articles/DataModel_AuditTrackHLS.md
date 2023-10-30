# Audit tracking for Clinic, Patient and Hospital

## Problem description and design
Track audit records for patients, clinics and hospital. <br/>
Sample audit records:

```
{
    "id": "GUID1", // TransactionId
    "entityType": "Clinic" // [Clinic/Patient/Hospital]
    "eventType": "Clinic_Data",
    "eventSource": "Application001",
    "entityAction": "Delete",
    "actionDate": 2023-10-30,
    "actionTimestamp": 1698673033,
    "clinicId": "12345678",
    "payload": {
        // "Details about the event being audited"
    }
}

{
    "id": "GUID2", // TransactionId
    "entityType": "Patient" [Clinic/Patient/Hospital]
    "eventType": "Patient_Address",
    "eventSource": "MobileApp02",
    "entityAction": "Update",
    "actionDate": 2023-10-30,
    "actionTimestamp": 1698673050,
    "patientId": "987654321",
    "payload": {
        // "Details about the event being audited"
    }
}

{
    "id": "GUID3", // TransactionId
    "entityType": "Hospital" [Clinic/Patient/Hospital]
    "eventType": "Hospital_Staff",
    "eventSource": "ISVMultiTenantApp01",
    "entityAction": "Update",
    "actionDate": 2023-10-30,
    "actionTimestamp": 1698673090,
    "hospitalId": "11111111",
    "payload": {
        // "Details about the event being audited"
    }
}
```

Sample queries<br/>

To fetch clinic audit history:<br/>```SELECT .... FROM c WHERE c.entityType = 'Clinic' and c.clinicId = @clinicId```

To fetch patient audit history:<br/>```SELECT ... FROM c WHERE c.entityType = 'Patient' and c.patientId = @patientId```

<br/>

> What would be your choice or partition key?
<br/>

## 1st design
Developers decided to use entityType as partition key.<br/>
After testing their solution in lower environments, they noticed that the partition key was not evenly distributed and ended up reaching the 20 GB logical partition key limit (https://learn.microsoft.com/en-us/azure/cosmos-db/partitioning-overview#logical-partitions) when registering many changes for different entities, as all patient audit documents will be under the same partition. <br/>

## Suggested design

Create a generic entityId property, which is a copy of the relevant id (clinicId, patientId, hospitalId) and use it as partition key. <br/>
Keep id as GUID with generic transactionId. All other properties remain the same. <br/>

Sample audit records:
```
{
    "id": "GUID1", // TransactionId
    "entityId": "12345678",
    "entityType": "Clinic" // [Clinic/Patient/Hospital]
    "eventType": "Clinic_Data",
    "eventSource": "Application001",
    "entityAction": "Delete",
    "actionDate": 2023-10-30,
    "actionTimestamp": 1698673033,
    "clinicId": "12345678",
    "payload": {
        // "Details about the event being audited"
    }
}

{
    "id": "GUID2", // TransactionId
    "entityId": "987654321",
    "entityType": "Patient" [Clinic/Patient/Hospital]
    "eventType": "Patient_Address",
    "eventSource": "MobileApp02",
    "entityAction": "Update",
    "actionDate": 2023-10-30,
    "actionTimestamp": 1698673050,
    "patientId": "987654321",
    "payload": {
        // "Details about the event being audited"
    }
}

{
    "id": "GUID3", // TransactionId
    "entityId": "11111111",
    "entityType": "Hospital" [Clinic/Patient/Hospital]
    "eventType": "Hospital_Staff",
    "eventSource": "ISVMultiTenantApp01",
    "entityAction": "Update",
    "actionDate": 2023-10-30,
    "actionTimestamp": 1698673090,
    "hospitalId": "11111111",
    "payload": {
        // "Details about the event being audited"
    }
}
```

The queries will need to be adjusted to use the entityId property to avoid cross partition queries. For example: <br/>

To fetch clinic audit history:<br/>```SELECT .... FROM c WHERE c.entityType = 'Clinic' and c.entityId = @clinicId```

To fetch patient audit history:<br/>```SELECT ... FROM c WHERE c.entityType = 'Patient' and c.entityId = @patientId```

To fetch patient audit history:<br/>```SELECT ... FROM c WHERE c.entityType = 'Hospital' and c.entityId = @hospitalId```

Even if business requires the patient document to associated with a specific clinic, the query can be modified to filter by entityId (patientId) and clinicI, still being a single partition query. For example:<br/>```SELECT ... FROM c WHERE c.entityType = 'Patient' and c.entityId = @patientId and c.clinicId = @clinicId```

Unless the business has a specific hospital/clinic with many documents, the partition key will be evenly distributed and the 20 GB limit will not be reached.
Usually this type of workload is also associated with a TTL, allowing hot data to be queried and moved to a different storage (cold storage) after a period of time.

