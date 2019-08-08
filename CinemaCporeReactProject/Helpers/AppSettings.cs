using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CinemaCporeReactProject.Helpers
{

    public class AppSettings
    {
        public string Secret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }

        public SymmetricSecurityKey GenerateKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        }
    }
}
