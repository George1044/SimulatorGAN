
using UnityEngine;
using System;
using System.Net;
using RESTfulHTTPServer.src.models;
using RESTfulHTTPServer.src.controller;


namespace RESTfulHTTPServer.src.invoker
{
    public class CoroutineExecuter : MonoBehaviour { }
    public class GenerateMapInvoke : MonoBehaviour
    {
        private const string TAG = "Gemerate Map Invoke";
        private static CoroutineExecuter instance;

        public static Response Generate(Request request)
        {
            Debug.Log("GENERATING");
            Response response = new Response();
            string responseData = "";
            string json = request.GetPOSTData();
            bool valid = true;

            UnityInvoker.ExecuteOnMainThread.Enqueue(() =>
            {

                try
                {
                    MapInvokerRequest jsonObj = MapInvokerRequest.CreateFromJSON(json);
                    string matrixString = jsonObj.matrixString;
                    ConvertMatrixToMap generator = GameObject.Find("Grid/FactoryMap").GetComponent<ConvertMatrixToMap>();
                    generator.Generate(matrixString);
                    if (!instance)
                    {
                        instance = FindObjectOfType<CoroutineExecuter>();
                        if (!instance)
                        {
                            instance = new GameObject("CoroutineExecuter").AddComponent<CoroutineExecuter>();
                        }
                    }
                    instance.StartCoroutine(generator.WaitTillSimulationComplete((isDone) =>
                    {
                        Debug.Log("COROUTINE IN POST");
                        if (isDone)
                        {
                            Debug.Log("COROUTINE COMPLETE IN POST");
                            GoalReached goalReached = generator.goal.GetComponent<GoalReached>();
                            SimulationResponse simulationResult = new SimulationResponse(goalReached.timer, goalReached.collisionCount);
                            print(simulationResult);
                            response.SetHTTPStatusCode((int)HttpStatusCode.OK);
                            responseData = JsonUtility.ToJson(simulationResult);
                            print(responseData);
                        }
                    }));

                }
                catch (Exception ex)
                {
                    valid = false;
                    string msg = "failed to deseiralised JSON";
                    responseData = msg;
                }

            });

            // Wait for the main thread
            while (responseData.Equals("")) ;
            print(responseData);

            // Filling up the response with data
            if (valid)
            {

                // 200 - OK
                response.SetContent(responseData);
                response.SetHTTPStatusCode((int)HttpStatusCode.OK);
                response.SetMimeType(Response.MIME_CONTENT_TYPE_JSON);
            }
            else
            {

                // 406 - Not acceptable
                response.SetContent("Somthing wrong");
                response.SetHTTPStatusCode((int)HttpStatusCode.NotAcceptable);
                response.SetMimeType(Response.MIME_CONTENT_TYPE_HTML);
            }
            return response;
        }
    }
}