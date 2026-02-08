import json
import uuid
import hashlib
from faker import Faker
import random
from decimal import Decimal
from datetime import datetime

fake = Faker()

def get_property_type(index: int) -> str:
    """
    Get consistent property type based on index using modulo.
    Ensures the same property index always has the same type.
    Distribution: 60% strings, 15% dates, 15% decimals, 5% booleans, 5% integers
    
    Args:
        index (int): The property index (0-99)
        
    Returns:
        str: The data type for this property index
    """
    # Create a deterministic mapping using weighted distribution
    # 100 properties total: 60 strings, 15 dates, 15 decimals, 5 booleans, 5 integers
    type_mapping = (
        ["string"] * 60 +
        ["date"] * 15 +
        ["decimal"] * 15 +
        ["boolean"] * 5 +
        ["integer"] * 5
    )
    return type_mapping[index % 100]



def generate_person_entity(entity_id: str, generate_large_payload: bool = False) -> dict:
    """
    Generate a sample Person entity document using Faker library.
    
    Args:
        entity_id (str): The entityId to use for linking documents
        generate_large_payload (bool): If True, add 100 properties with random values
        
    Returns:
        dict: A dictionary representing a Person entity document
    """
    first_name = fake.first_name()
    last_name = fake.last_name()
    middle_name = fake.first_name() if random.choice([True, False]) else ""
    
    # Create full name with optional middle name
    full_name = f"{first_name} {middle_name} {last_name}" if middle_name else f"{first_name} {last_name}"
    
    # Generate SSN and create a simple hash
    ssn = fake.ssn()
    ssn_hash = hashlib.md5(ssn.encode()).hexdigest()[:6] + "****"
    
    document = {
        "id": str(uuid.uuid4()),
        "entityId": entity_id,
        "entityType": "Entity",
        "version": 1.0,
        "firstName": first_name,
        "lastName": last_name,
        "fullName": full_name,
        "dateOfBirth": fake.date_of_birth(minimum_age=18, maximum_age=80).strftime("%Y-%m-%d"),
        "gender": random.choice(["M", "F"]),
        "ssn": ssn,
        "ssnHash": ssn_hash,
        "issuingState": fake.state_abbr()
    }
    
    # Add 100 properties with random values if large payload is requested
    if generate_large_payload:
        for i in range(100):
            prop_name = f"property_{i}"
            # Get consistent type for this property index
            value_type = get_property_type(i)
            
            if value_type == "string":
                document[prop_name] = fake.word() + " " + fake.word()
            elif value_type == "date":
                document[prop_name] = fake.date_time().isoformat()
            elif value_type == "decimal":
                document[prop_name] = float(Decimal(str(round(random.uniform(1, 10000), 2))))
            elif value_type == "boolean":
                document[prop_name] = random.choice([True, False])
            else:  # integer
                document[prop_name] = random.randint(1, 100000)
    
    return document

def generate_person_contact_info(entity_id: str) -> dict:
    """
    Generate a sample PersonContactInfo document using Faker library.
    
    Args:
        entity_id (str): The entityId to use for linking documents
        
    Returns:
        dict: A dictionary representing a PersonContactInfo document
    """
    
    def generate_address():
        """Helper function to generate an address object"""
        return {
            "street": fake.street_address(),
            "city": fake.city(),
            "state": fake.state_abbr(),
            "zipCode": fake.zipcode(),
            "Country": "USA"
        }
    
    def generate_phone():
        """Helper function to generate a phone object"""
        phone_types = ["Mobile", "Home", "Work", "Fax"]
        return {
            "phoneNumber": fake.phone_number(),
            "phoneType": random.choice(phone_types)
        }
    
    def generate_email():
        """Helper function to generate an email object"""
        email_types = ["Personal", "Professional", "Work", "Other"]
        return {
            "emailAddress": fake.email(),
            "emailType": random.choice(email_types)
        }
    
    # Generate main address
    main_address = generate_address()
    
    # Generate 1-3 other addresses
    other_addresses = [generate_address() for _ in range(random.randint(0, 3))]
    
    # Generate main phone
    main_phone = generate_phone()
    
    # Generate 0-2 other phones
    other_phones = [generate_phone() for _ in range(random.randint(0, 2))]
    
    # Generate main email
    main_email = generate_email()
    
    # Generate 0-2 other emails
    other_emails = [generate_email() for _ in range(random.randint(0, 2))]
    
    document = {
        "id": str(uuid.uuid4()),
        "entityId": entity_id,
        "entityType": "PersonContactInfo",
        "version": 1.0,
        **main_address,  # Spread the main address fields at the root level
        "otherAddresses": other_addresses,
        "phoneNumber": main_phone["phoneNumber"],
        "phoneType": main_phone["phoneType"],
        "otherPhones": other_phones,
        "emailAddress": main_email["emailAddress"],
        "emailType": main_email["emailType"],
        "otherEmails": other_emails
    }
    
    return document

def generate_linked_documents():
    """
    Generate a pair of linked documents with the same entityId.
    
    Returns:
        tuple: A tuple containing (person_entity, person_contact_info)
    """
    shared_entity_id = str(uuid.uuid4())
    
    person_entity = generate_person_entity(shared_entity_id)
    person_contact_info = generate_person_contact_info(shared_entity_id)
    
    return person_entity, person_contact_info

# Example usage
if __name__ == "__main__":
    # Generate documents with the same entityId
    entity_id = str(uuid.uuid4())
    
    person_doc = generate_person_entity(entity_id, generate_large_payload=True)
    contact_doc = generate_person_contact_info(entity_id)
    
    print("Person Entity Document:")
    print(json.dumps(person_doc, indent=2))
    print("\n" + "="*50 + "\n")
    print("Person Contact Info Document:")
    print(json.dumps(contact_doc, indent=2))
    
    # Alternative: Generate linked documents using the helper function
    print("\n" + "="*50 + "\n")
    print("Using generate_linked_documents():")
    person, contact = generate_linked_documents()
    print(f"Both documents share entityId: {person['entityId']}")


