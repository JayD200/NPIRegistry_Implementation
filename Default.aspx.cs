using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace NPIRegistry_Implementation
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            resultContainer.InnerHtml = string.Empty;

            var npi = txtNPI.Text.Trim();
            if (string.IsNullOrWhiteSpace(npi) || !Regex.IsMatch(npi, @"^\d{10}$"))
            {
                resultContainer.InnerHtml = "<div class='alert alert-danger'>Please enter a valid 10-digit NPI.</div>";
                return;
            }

            var apiUrl = "https://npiregistry.cms.hhs.gov/api/?number=" + npi + "&version=2.1";

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UserAgent = "NPIRegistry_Implementation";

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        resultContainer.InnerHtml = "<div class='alert alert-danger'>Unable to retrieve NPI data.</div>";
                        return;
                    }

                    resultContainer.InnerHtml = FormatProviderOutput(json);
                }
            }
            catch (Exception ex)
            {
                resultContainer.InnerHtml = "<div class='alert alert-danger'>Error retrieving data: " + HttpUtility.HtmlEncode(ex.Message) + "</div>";
            }
        }

        private string FormatProviderOutput(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return "<div class='alert alert-danger'>No provider data found.</div>";
            }

            var serializer = new JavaScriptSerializer();
            var root = serializer.DeserializeObject(json) as Dictionary<string, object>;
            if (root == null || !root.ContainsKey("results"))
            {
                return "<div class='alert alert-danger'>No provider data found.</div>";
            }

            var results = root["results"] as object[];
            if (results == null || results.Length == 0)
            {
                return "<div class='alert alert-danger'>No provider data found.</div>";
            }

            var provider = results[0] as Dictionary<string, object>;
            if (provider == null)
            {
                return "<div class='alert alert-danger'>No provider data found.</div>";
            }

            var basic = provider.ContainsKey("basic") ? provider["basic"] as Dictionary<string, object> : null;
            var taxonomies = provider.ContainsKey("taxonomies") ? provider["taxonomies"] as object[] : null;

            var sb = new StringBuilder();
            sb.Append("<div class='provider-output' style='line-height: 1.8;'>");

            if (basic != null)
            {
                var firstName = GetStringValue(basic, "first_name");
                var middleName = GetStringValue(basic, "middle_name");
                var lastName = GetStringValue(basic, "last_name");
                var credential = GetStringValue(basic, "credential");

                var displayName = BuildDisplayName(firstName, middleName, lastName, credential);

                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    sb.Append("<div><strong>Provider:</strong> ");
                    sb.Append(HttpUtility.HtmlEncode(displayName));
                    sb.Append("</div>");
                }

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    sb.Append("<div><strong>First Name:</strong> ");
                    sb.Append(HttpUtility.HtmlEncode(firstName));
                    sb.Append("</div>");
                }

                if (!string.IsNullOrWhiteSpace(middleName))
                {
                    sb.Append("<div><strong>Middle Name:</strong> ");
                    sb.Append(HttpUtility.HtmlEncode(middleName));
                    sb.Append("</div>");
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    sb.Append("<div><strong>Last Name:</strong> ");
                    sb.Append(HttpUtility.HtmlEncode(lastName));
                    sb.Append("</div>");
                }

                if (!string.IsNullOrWhiteSpace(credential))
                {
                    sb.Append("<div><strong>Credential:</strong> ");
                    sb.Append(HttpUtility.HtmlEncode(credential));
                    sb.Append("</div>");
                }
            }

            if (taxonomies != null && taxonomies.Length > 0)
            {
                sb.Append("<div style='margin-top: 12px;'><strong>Taxonomies:</strong></div>");
                sb.Append("<ul>");

                foreach (var taxonomyItem in taxonomies)
                {
                    var taxonomy = taxonomyItem as Dictionary<string, object>;
                    if (taxonomy != null)
                    {
                        var description = GetStringValue(taxonomy, "desc");
                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            sb.Append("<li>");
                            sb.Append(HttpUtility.HtmlEncode(description));
                            sb.Append("</li>");
                        }
                    }
                }

                sb.Append("</ul>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }

        private string GetStringValue(Dictionary<string, object> item, string key)
        {
            if (item == null || !item.ContainsKey(key))
            {
                return string.Empty;
            }

            var value = item[key];
            return value == null ? string.Empty : value.ToString();
        }

        private string BuildDisplayName(string firstName, string middleName, string lastName, string credential)
        {
            var nameParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(credential))
            {
                var normalizedCredential = credential.Trim();
                if (Regex.IsMatch(normalizedCredential, @"^(MD|DO|M\.D\.|D\.O\.)$", RegexOptions.IgnoreCase))
                {
                    nameParts.Add("Dr");
                }
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                nameParts.Add(firstName.Trim());
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                nameParts.Add(middleName.Trim());
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                nameParts.Add(lastName.Trim());
            }

            var displayName = string.Join(" ", nameParts);

            if (!string.IsNullOrWhiteSpace(credential))
            {
                var trimmedCredential = credential.Trim();
                if (!string.IsNullOrEmpty(displayName))
                {
                    displayName += ", " + trimmedCredential;
                }
                else
                {
                    displayName = trimmedCredential;
                }
            }

            return displayName;
        }
    }
}
