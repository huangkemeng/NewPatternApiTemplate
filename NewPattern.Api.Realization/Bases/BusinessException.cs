using System.ComponentModel;
using System.Reflection;

namespace NewPattern.Api.Realization.Bases
{
    public class BusinessException : Exception
    {
        public BusinessException(string msg, BusinessExceptionTypeState exceptionType = BusinessExceptionTypeState.NotSpecified) : base(GetFullExceptionMessage(exceptionType, msg))
        {

        }
        public BusinessException(IEnumerable<string> msg, BusinessExceptionTypeState exceptionType = BusinessExceptionTypeState.NotSpecified) : base(GetFullExceptionMessage(exceptionType, msg.ToArray()))
        {

        }


        private static string GetFullExceptionMessage(BusinessExceptionTypeState exceptionType, params string[] msg)
        {
            var businessExceptionTypeStateType = typeof(BusinessExceptionTypeState);
            var businessExceptionTypeStateTypeField = businessExceptionTypeStateType.GetField(exceptionType.ToString())!;
            var descriptionAttr = businessExceptionTypeStateTypeField.GetCustomAttribute(typeof(DescriptionAttribute));
            if (descriptionAttr is DescriptionAttribute description)
            {
                return $"{description.Description}：{string.Join(";", msg)}";
            }
            return $"{exceptionType}：{msg}";
        }
    }
    public enum BusinessExceptionTypeState
    {
        [Description("")] NotSpecified,
        [Description("参数有误")] Validator,
    }


}
