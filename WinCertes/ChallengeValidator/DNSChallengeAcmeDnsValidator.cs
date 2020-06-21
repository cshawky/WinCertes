using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WinCertes;

namespace WinCertes.ChallengeValidator
{
    class DNSChallengeAcmeDnsValidator : IDNSChallengeValidator
    {
        private WinCertesOptions _options;

        /// <summary>
        /// Constructor for the ACME-DNS DNS challenge validator
        /// </summary>
        /// <param name="config"></param>
        public DNSChallengeAcmeDnsValidator()
        {
            _options = Program._winCertesOptions;
        }

        /// <summary>
        /// Preparing ACME-DNS by sending ACME DNS token using ACME-DNS credentials and API.
        /// </summary>
        /// <param name="dnsKeyName"></param>
        /// <param name="dnsKeyValue"></param>
        /// <returns>True if challenge was successful</returns>
        public bool PrepareChallengeForValidation(string dnsKeyName, string dnsKeyValue)
        {
            try
            {
                var DNSServerURL = _options.DNSServerURL;
                var DNSServerUser = _options.DNSServerUser;
                var DNSServerKey = _options.DNSServerKey;
                var DNSServerSubDomain = _options.DNSServerSubDomain;

                if (!Uri.IsWellFormedUriString(DNSServerURL, UriKind.Absolute)) 
                    return false;
                HttpClient client = new HttpClient();
                var content = new StringContent($"{{ \"subdomain\": \"{DNSServerSubDomain}\", \"txt\": \"{dnsKeyValue}\" }}", Encoding.UTF8, "application/json");
                content.Headers.Add("X-Api-User", DNSServerUser);
                content.Headers.Add("X-Api-Key", DNSServerKey);

                var response = client.PostAsync(DNSServerURL, content).Result;
                return (response.StatusCode == System.Net.HttpStatusCode.OK);
            } 
            catch (Exception exp) {
                Program._logger.Error($"PrepareChallengeForValidation: {exp.Message}");
            }
            return false;
        }
    }
}
