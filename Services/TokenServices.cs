using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
namespace PortofolioApi.Services;

    public class TokenServices
    {
        private string _token;
        private string _role;
        private string _userId; // <-- nouveau champ pour l'ID

        public void SetToken(string token)
        {
            _token = token;
            _role = ExtractRoleFromToken(token);
            _userId = ExtractIdFromToken(token); // <-- extraction de l'ID
        }

        public string GetToken() => _token;
        public string GetRole() => _role;
        public string GetUserId() => _userId; // <-- méthode pour récupérer l'ID

        public void ClearToken()
        {
            _token = null;
            _role = null;
            _userId = null;
        }

        private string ExtractRoleFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            foreach (var claim in jwt.Claims)
            {
                if (claim.Type == "role")
                {
                    Console.WriteLine("Extracted role from token: " + claim.Value);
                    return claim.Value;
                }
            }
            return null;
        }

        private string ExtractIdFromToken(string token) // <-- nouvelle méthode
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            foreach (var claim in jwt.Claims)
            {
                if (claim.Type == "id") // <-- ici on récupère l'ID
                {
                    Console.WriteLine("Extracted ID from token: " + claim.Value);
                    return claim.Value;
                }
            }
            return null;
        }
    }

