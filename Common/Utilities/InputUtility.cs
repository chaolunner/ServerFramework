using System.Collections.Generic;

namespace Common
{
    public static class InputUtility
    {
        public static object FromInputData(InputData input)
        {
#if UNITY
            return UnityEngine.JsonUtility.FromJson(input.Data, TypeHelper.GetType(input.Name));
#else
            return JsonUtility.FromJson(input.Data, TypeHelper.GetType(input.Name));
#endif
        }

        public static InputData ToInputData<T>(T obj) where T : IInput
        {
            InputData inputData = new InputData();
            inputData.Name = obj.GetType().FullName;
#if UNITY
            inputData.Data = UnityEngine.JsonUtility.ToJson(obj);
#else
            inputData.Data = JsonUtility.ToJson(obj);
#endif
            return inputData;
        }

        public static InputData[] ToInputData<T>(T[] objs) where T : IInput
        {
            InputData[] inputData = new InputData[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                inputData[i] = ToInputData<T>(objs[i]);
            }
            return inputData;
        }

        public static InputData[] ToInputData<T>(List<T> objs) where T : IInput
        {
            InputData[] inputData = new InputData[objs.Count];
            for (int i = 0; i < objs.Count; i++)
            {
                inputData[i] = ToInputData<T>(objs[i]);
            }
            return inputData;
        }
    }
}
