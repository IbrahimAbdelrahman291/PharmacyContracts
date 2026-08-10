using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hashedPassword, string providedPassword);
    }
}
