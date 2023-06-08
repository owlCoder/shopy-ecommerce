using System;
using System.Reflection;

namespace PR55_2020_Danijel_Jovanovic.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}