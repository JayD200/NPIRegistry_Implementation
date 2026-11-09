using System.Collections.Generic;
using Newtonsoft.Json;

public class NpiResponse
{
    [JsonProperty("result_count")] public int ResultCount { get; set; }
    [JsonProperty("results")] public List<NpiResult> Results { get; set; }
    [JsonProperty("Errors")] public List<NpiError> Errors { get; set; }
}

public class NpiError
{
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("field")] public string Field { get; set; }
}

public class NpiResult
{
    [JsonProperty("number")] public long Number { get; set; }
    [JsonProperty("enumeration_type")] public string EnumerationType { get; set; }
    [JsonProperty("basic")] public NpiBasic Basic { get; set; }
    [JsonProperty("addresses")] public List<NpiAddress> Addresses { get; set; }
    [JsonProperty("taxonomies")] public List<NpiTaxonomy> Taxonomies { get; set; }
}

public class NpiBasic
{
    [JsonProperty("first_name")] public string FirstName { get; set; }
    [JsonProperty("last_name")] public string LastName { get; set; }
    [JsonProperty("organization_name")] public string OrganizationName { get; set; }
    [JsonProperty("credential")] public string Credential { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
}

public class NpiAddress
{
    [JsonProperty("address_purpose")] public string Purpose { get; set; }
    [JsonProperty("address_1")] public string Address1 { get; set; }
    [JsonProperty("city")] public string City { get; set; }
    [JsonProperty("state")] public string State { get; set; }
    [JsonProperty("postal_code")] public string PostalCode { get; set; }
    [JsonProperty("telephone_number")] public string Phone { get; set; }
}

public class NpiTaxonomy
{
    [JsonProperty("code")] public string Code { get; set; }
    [JsonProperty("desc")] public string Description { get; set; }
    [JsonProperty("primary")] public bool Primary { get; set; }
}