using System;

namespace BLL_Project.Dtos
{
    public class TokenDto
    {
        public TokenDto(string Token,DateTime ExpireDate) {
            this.Token = Token;
            this.ExpireDate = ExpireDate;
        }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
