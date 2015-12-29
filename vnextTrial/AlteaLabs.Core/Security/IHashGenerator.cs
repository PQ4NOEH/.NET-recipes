using AlteaLabs.Core.Guards;

namespace AlteaLabs.Core.Security
{
    public interface IHashGenerator
    {
        NotNulllEmptyOrWhiteSpaceString GenerateHash(NotNulllEmptyOrWhiteSpaceString data);
    }
}