using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace farm2plate.AWServices
{
    public class SNSService
    {
        AmazonSimpleNotificationServiceClient SNSClient;
        ListTopicsResponse ListTopicsResponse;
        ListTopicsRequest ListTopicsRequest;
        string _topicarn;

        public SNSService() {
            SNSClient = new AmazonSimpleNotificationServiceClient();
            _topicarn = GetTopic().Result;
        }

        // Constraint: we're only grabbing the first ARN
        async Task<string> GetTopic() {
            ListTopicsRequest = new ListTopicsRequest();
            ListTopicsResponse = new ListTopicsResponse();
            ListTopicsResponse = await SNSClient.ListTopicsAsync(ListTopicsRequest);
            if (ListTopicsResponse.Topics.Count != 0) {
                string topicarn = ListTopicsResponse.Topics.First().TopicArn;
                System.Diagnostics.Debug.WriteLine($"ARN FOUND: {topicarn}");
                return topicarn;
            }
            return null;
        }

        public async void AddToSandbox(string number) {
            CreateSMSSandboxPhoneNumberRequest sandboxRequest = new CreateSMSSandboxPhoneNumberRequest {
                PhoneNumber = number,
            };
            try {
                CreateSMSSandboxPhoneNumberResponse sandboxResponse = await SNSClient.CreateSMSSandboxPhoneNumberAsync(sandboxRequest);
                System.Diagnostics.Debug.WriteLine($"SANDBOX ADD SUCCESS: {sandboxResponse.HttpStatusCode}");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"SANDBOX ADD FAILED: {ex.Message}");
            }
        }

        public async Task<bool> IsNumberConfirmed(string number) {
            ListSMSSandboxPhoneNumbersRequest sandboxRequest = new ListSMSSandboxPhoneNumbersRequest();
            try {
                ListSMSSandboxPhoneNumbersResponse sandboxResponse = await SNSClient.ListSMSSandboxPhoneNumbersAsync(sandboxRequest);
                foreach (SMSSandboxPhoneNumber phone in sandboxResponse.PhoneNumbers) {
                    if (phone.PhoneNumber.Contains(number) && phone.Status == SMSSandboxPhoneNumberVerificationStatus.Verified) {
                        return true;
                    }
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"SANDBOX CONFIRMATION CHECK FAILED: {ex.Message}");
            }
            return false;

        }

        public async void ConfirmSandbox(string otp, string phone) {
            VerifySMSSandboxPhoneNumberRequest sandboxRequest = new VerifySMSSandboxPhoneNumberRequest {
                OneTimePassword = otp,
                PhoneNumber = phone
            };
            try {
                VerifySMSSandboxPhoneNumberResponse sandboxResponse = await SNSClient.VerifySMSSandboxPhoneNumberAsync(sandboxRequest);
                System.Diagnostics.Debug.WriteLine($"SANDBOX CONFIRM SUCCESS: {sandboxResponse.HttpStatusCode}");
            } 
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"SANDBOX CONFIRM FAILED: {ex.Message}");
            }
        }

        public async void AddSubscriberSMS(string number) {
            SubscribeRequest subscribeRequest = new SubscribeRequest(
                topicArn: _topicarn,
                protocol: "SMS",
                endpoint: number);
            try {
                SubscribeResponse subscribeResponse = await SNSClient.SubscribeAsync(subscribeRequest);
                System.Diagnostics.Debug.WriteLine($"SUBSCRIBE SUCCESS: {subscribeResponse.HttpStatusCode}");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"SUBSCRIBE FAILED: {ex.Message}");
            }
        }

        public async void SendSMS(string number, string message) {
            PublishRequest request = new PublishRequest {
                Message = message,
                PhoneNumber = number,
            };
            try {
                PublishResponse response = await SNSClient.PublishAsync(request);
                System.Diagnostics.Debug.WriteLine($"SMS SENT: {response.HttpStatusCode}");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"SMS FAILED: {ex.Message}");
            }
        }

        public async void SendEmail(string message) {
            var request = new PublishRequest {
                Message = message,
                TopicArn = _topicarn
            };

            try {
                var response = await SNSClient.PublishAsync(request);
                System.Diagnostics.Debug.WriteLine($"EMAIL SENT: {response.HttpStatusCode}");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"EMAIL FAILED: {ex.Message}");
            }
        }
    }
}
