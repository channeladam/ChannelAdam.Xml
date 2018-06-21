Feature: XML Serialisation

################################
# Serialisation
################################
Scenario: Should correctly serialise an object - with no changes to the XML attributes
Given an object with a default XML root attribute
When the object with a default XML root attribute is serialised with no serialisation overrides
Then the object is serialised correctly

Scenario: Should correctly serialise an object - when overriding the XML root attribute
Given an object with a default XML root attribute
When the object with a default XML root attribute is serialised with an override of the XML root attribute
Then the object is serialised correctly

Scenario: Should correctly serialise an object - when overriding the XML attributes
Given an object with a default XML root attribute
When the object with a default XML root attribute is serialised with an override of the XML attributes
Then the object is serialised correctly

Scenario: Should correctly serialise an instance that has been cast as an object
Given a test object
When the instance cast as an object is serialised
Then the object is serialised correctly

################################
# Deserialisation
################################
Scenario: Should correctly deserialise an object - with no changes to the XML attributes
Given a class with a default XML root attribute
And an XML string with the default XML root attribute
When the XML string with the default XML root attribute is deserialised with no serialisation overrides
Then the XML string is deserialised successfully

Scenario: Should correctly deserialise an object - when overriding the XML root attribute
Given a class with a default XML root attribute
And an XML string with a root attribute that is different from the default XML root attribute
When the XML string with the different XML root attribute is deserialised with an override of the XML root attribute
Then the XML string is deserialised successfully

Scenario: Should correctly deserialise an object - when overriding the XML attributes
Given a class with a default XML root attribute
And an XML string with a root attribute that is different from the default XML root attribute
When the XML string with the different XML root attribute is deserialised with an override of the XML attributes
Then the XML string is deserialised successfully
