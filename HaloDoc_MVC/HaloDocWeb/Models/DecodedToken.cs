using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace HaloDocWeb.Models
{
    public class DecodedToken : Controller
    {
                private List<string> audience1;
        public Dictionary<string, string> claims;
        private DateTime validTo;
        private string signatureAlgorithm;
        private string rawData;
        private string subject;
        private DateTime validFrom;
        private string encodedHeader;
        private string encodedPayload;
        private string keyId;

        public DecodedToken(string keyId, string issuer, List<string> audience1, Dictionary<string, string> claims, DateTime validTo, string signatureAlgorithm, string rawData, string subject, DateTime validFrom, string encodedHeader, string encodedPayload)
        {
            this.keyId = keyId;
            issuer = issuer;
            this.audience1 = audience1;
            this.claims = claims;
            this.validTo = validTo;
            this.signatureAlgorithm = signatureAlgorithm;
            this.rawData = rawData;
            this.subject = subject;
            this.validFrom = validFrom;
            this.encodedHeader = encodedHeader;
            this.encodedPayload = encodedPayload;
        }
        public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }
        public static DecodedToken DecodeJwt(JwtSecurityToken token)
        {
            var keyId = token.Header.Kid;
            var audience = token.Audiences.ToList();
            var claims = token.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

            return new DecodedToken(
                keyId,
                token.Issuer,
                audience,
                claims,
                token.ValidTo,
                token.SignatureAlgorithm,
                token.RawData,
                token.Subject,
                token.ValidFrom,
                token.EncodedHeader,
                token.EncodedPayload
            );
        }
    }
}
