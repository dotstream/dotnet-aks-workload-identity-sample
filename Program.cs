using k8s;
using System.Text;


var server = Environment.GetEnvironmentVariable("DOTNET_K8S_SERVER");
var k8sNamespace = Environment.GetEnvironmentVariable("DOTNET_K8S_NS");
var kubelogin = Environment.GetEnvironmentVariable("DOTNET_KUBELOGIN_PATH") ?? "kubelogin";

var clientid = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
var authority_host = Environment.GetEnvironmentVariable("AZURE_AUTHORITY_HOST");
var federatedTokenFile = Environment.GetEnvironmentVariable("AZURE_FEDERATED_TOKEN_FILE");
var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");

Console.WriteLine($"DOTNET_K8S_SERVER={server}");
Console.WriteLine($"DOTNET_K8S_NS={k8sNamespace}");
Console.WriteLine($"DOTNET_KUBELOGIN_PATH={kubelogin}");
Console.WriteLine($"AZURE_CLIENT_ID={clientid}");
Console.WriteLine($"AZURE_AUTHORITY_HOST={authority_host}");
Console.WriteLine($"AZURE_FEDERATED_TOKEN_FILE={federatedTokenFile}");
Console.WriteLine($"AZURE_TENANT_ID={tenantId}");

using var configstream = new MemoryStream(Encoding.ASCII.GetBytes($"""
apiVersion: v1
clusters:
- cluster:
    insecure-skip-tls-verify: true
    server: {server}
  name: aks
contexts:
- context:
    cluster: aks
    user: msi
  name: aks
current-context: aks
kind: Config
users:
- name: msi
  user:
    exec:
      apiVersion: client.authentication.k8s.io/v1beta1
      args:
      - get-token
      - --login
      - workloadidentity
      - --server-id
      - 6dae42f8-4368-4678-94ff-3960e28e3630
      - --client-id
      - {clientid}
      - --authority-host
      - {authority_host}
      - --federated-token-file
      - {federatedTokenFile}
      - --tenant-id
      - {tenantId}
      command: {kubelogin}
      env: null
"""));

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(configstream);
IKubernetes client = new Kubernetes(config);

Console.WriteLine("Starting Request!");

var list = client.CoreV1.ListNamespacedPod(k8sNamespace);
foreach (var item in list.Items)
{
    Console.WriteLine(item.Metadata.Name);
}