using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Vision;
using CoreML;
using CoreImage;
using CoreFoundation;

[assembly: Xamarin.Forms.Dependency(typeof(CoreMLLabApp.iOS.FriesOrNotFriesService))]
namespace CoreMLLabApp.iOS
{
    public class FriesOrNotFriesService : IFriesOrNotFriesService
    {
        private static VNCoreMLModel VModel { get; }

        static FriesOrNotFriesService()
        {
            // Load the ML model
            var assetPath = NSBundle.MainBundle.GetUrlForResource("e3e4e645c0944c6ca84f9a000e501b22", "mlmodelc");
            var friedOrNotFriedModel = MLModel.Create(assetPath, out _);
            VModel = VNCoreMLModel.FromMLModel(friedOrNotFriedModel, out _);
        }

        public Task<string> DetectAsync(byte[] image)
        {
            var taskSource = new TaskCompletionSource<string>();
            void handleClassification(VNRequest request, NSError error)
            {
                var observations = request.GetResults<VNClassificationObservation>();
                if (observations == null)
                {
                    taskSource.SetException(new Exception("Unexpected result type from VNCoreMLRequest"));
                    return;
                }

                if (observations.Length == 0)
                {
                    taskSource.SetResult(null);
                    return;
                }

                var best = observations.First();
                taskSource.SetResult(best.Identifier);
            }

            using (var data = NSData.FromArray(image))
            {
                var ciImage = new CIImage(data);
                var handler = new VNImageRequestHandler(ciImage, new VNImageOptions());
                DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
                {
                    handler.Perform(new VNRequest[] { new VNCoreMLRequest(VModel, handleClassification) }, out _);
                });
            }

            return taskSource.Task;
        }

    }
}