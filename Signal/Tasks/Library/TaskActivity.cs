using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Tasks.Library
{
    /// <summary>
    ///     Base class for TaskActivity.
    ///     User activity should almost always derive from either TypedTaskActivity
    ///     <TInput, TResult> or TaskActivity<TInput, TResult>
    /// </summary>
    public abstract class TaskActivity
    {
        public abstract string Run(TaskContext context, string input);

        public virtual Task<string> RunAsync(TaskContext context, string input)
        {
            return Task.FromResult(Run(context, input));
        }


    }

    /// <summary>
    ///     Typed base class for creating typed async task activities
    /// </summary>
    /// <typeparam name="TInput">Input type for the activity</typeparam>
    /// <typeparam name="TResult">Output type of the activity</typeparam>
    public abstract class AsyncTaskActivity<TInput, TResult> : TaskActivity
    {
        /*protected TaskActivity()
        {
            DataConverter = new JsonDataConverter();
        }

        protected TaskActivity(DataConverter dataConverter)
        {
            if (dataConverter != null)
            {
                DataConverter = dataConverter;
            }
            else
            {
                DataConverter = new JsonDataConverter();
            }
        }

        public DataConverter DataConverter { get; protected set; }
        */
        public override string Run(TaskContext context, string input)
        {
            // will never run
            return string.Empty;
        }

        protected abstract Task<TResult> ExecuteAsync(TaskContext context, TInput input);

        public override async Task<string> RunAsync(TaskContext context, string input)
        {
            TInput parameter = default(TInput);
            /*JArray jArray = JArray.Parse(input);
            if (jArray != null)
            {
                int parameterCount = jArray.Count;
                if (parameterCount > 1)
                {
                    throw new TaskFailureException(
                        "TaskActivity implementation cannot be invoked due to more than expected input parameters.  Signature mismatch.");
                }

                if (parameterCount == 1)
                {
                    JToken jToken = jArray[0];
                    var jValue = jToken as JValue;
                    if (jValue != null)
                    {
                        parameter = jValue.ToObject<TInput>();
                    }
                    else
                    {
                        string serializedValue = jToken.ToString();
                        parameter = DataConverter.Deserialize<TInput>(serializedValue);
                    }
                }
            }*/

            TResult result;
            try
            {
                result = await ExecuteAsync(context, parameter);
            }
            catch (Exception e)
            {
                //string details = Utils.SerializeCause(e, DataConverter);
                throw new /*TaskFailure*/Exception(e.Message/*, details*/);
            }

            return "ASD";

            /*string serializedResult = DataConverter.Serialize(result);
            return serializedResult;*/
        }
    }

    /// <summary>
    ///     Typed base class for creating typed sync task activities
    /// </summary>
    /// <typeparam name="TInput">Input type for the activity</typeparam>
    /// <typeparam name="TResult">Output type of the activity</typeparam>
    public abstract class TaskActivity<TInput, TResult> : AsyncTaskActivity<TInput, TResult>
    {
        protected abstract TResult Execute(TaskContext context, TInput input);

        protected override Task<TResult> ExecuteAsync(TaskContext context, TInput input)
        {
            return Task.FromResult(Execute(context, input));
        }
    }









    public abstract class TaskActivityAsync : TaskActivity
    {

        public override string Run(TaskContext context, string input)
        {
            // will never run
            return string.Empty;
        }

        protected abstract Task<string> ExecuteAsync();

        public override async Task<string> RunAsync(TaskContext context, string input)
        {
           
            try
            {
                /*result =*/ await ExecuteAsync();
            }
            catch (Exception e)
            {
                //string details = Utils.SerializeCause(e, DataConverter);
                throw new /*TaskFailure*/Exception(e.Message/*, details*/);
            }

            return "ASD";

            /*string serializedResult = DataConverter.Serialize(result);
            return serializedResult;*/
        }
    }


    public abstract class UntypedTaskActivity : TaskActivityAsync
    {
        public UntypedTaskActivity() {
        }

        protected abstract string Execute();

        protected override Task<string> ExecuteAsync()
        {
            return Task.FromResult(Execute());
        }


        public abstract void onAdded();

        public void OnCanceled()
        {
            
        }

    }



}
