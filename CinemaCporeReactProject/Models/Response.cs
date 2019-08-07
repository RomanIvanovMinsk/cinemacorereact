namespace CinemaCporeReactProject.Models
{
    using System.Collections.Generic;

    namespace ReactGetStarted.Model
    {
        public class Response
        {
            public Response()
            {
                Errors = new List<Error>();
            }
            public bool IsSuccess { get; internal set; }
            public List<Error> Errors { get; }
        }

        public class Response<T> : Response
        {
            public T Data { get; internal set; }
        }

        public static class ResponseExtension
        {
            public static T AddError<T>(this T model, Error error) where T : Response
            {
                model.Errors.Add(error);
                model.IsSuccess = false;
                return model;
            }

            public static T Success<T>(this T model) where T : Response
            {
                model.IsSuccess = true;
                return model;
            }

            public static T Success<T, D>(this T model, D data) where T : Response<D>
            {
                model.Data = data;
                model.IsSuccess = true;
                return model;
            }
        }

        public class Error
        {
            public Error(string key, string message, string code = null)
            {
                Key = key;
                Message = message;
                Code = code;
            }
            public string Key { get; internal set; }
            public string Message { get; internal set; }
            public string Code { get; internal set; }
        }
    }

}
