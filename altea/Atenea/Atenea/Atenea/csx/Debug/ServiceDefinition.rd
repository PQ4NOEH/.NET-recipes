<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Atenea" generation="1" functional="0" release="0" Id="6df639d0-30a9-4c44-823f-7fd3ed4e89c2" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AteneaGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Atenea.Worker:AlteaTcp" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Atenea/AteneaGroup/LB:Atenea.Worker:AlteaTcp" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Atenea.Worker:Microsoft.ServiceBus.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.Worker:Microsoft.ServiceBus.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="Atenea.Worker:Microsoft.ServiceBus.QueueName" defaultValue="">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.Worker:Microsoft.ServiceBus.QueueName" />
          </maps>
        </aCS>
        <aCS name="Atenea.Worker:Microsoft.ServiceBus.ServiceNamespace" defaultValue="">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.Worker:Microsoft.ServiceBus.ServiceNamespace" />
          </maps>
        </aCS>
        <aCS name="Atenea.Worker:Microsoft.ServiceBus.SharedAccessKey" defaultValue="">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.Worker:Microsoft.ServiceBus.SharedAccessKey" />
          </maps>
        </aCS>
        <aCS name="Atenea.Worker:Microsoft.ServiceBus.SharedAccessKeyName" defaultValue="">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.Worker:Microsoft.ServiceBus.SharedAccessKeyName" />
          </maps>
        </aCS>
        <aCS name="Atenea.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="Atenea.WorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Atenea/AteneaGroup/MapAtenea.WorkerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Atenea.Worker:AlteaTcp">
          <toPorts>
            <inPortMoniker name="/Atenea/AteneaGroup/Atenea.Worker/AlteaTcp" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapAtenea.Worker:Microsoft.ServiceBus.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Atenea/AteneaGroup/Atenea.Worker/Microsoft.ServiceBus.ConnectionString" />
          </setting>
        </map>
        <map name="MapAtenea.Worker:Microsoft.ServiceBus.QueueName" kind="Identity">
          <setting>
            <aCSMoniker name="/Atenea/AteneaGroup/Atenea.Worker/Microsoft.ServiceBus.QueueName" />
          </setting>
        </map>
        <map name="MapAtenea.Worker:Microsoft.ServiceBus.ServiceNamespace" kind="Identity">
          <setting>
            <aCSMoniker name="/Atenea/AteneaGroup/Atenea.Worker/Microsoft.ServiceBus.ServiceNamespace" />
          </setting>
        </map>
        <map name="MapAtenea.Worker:Microsoft.ServiceBus.SharedAccessKey" kind="Identity">
          <setting>
            <aCSMoniker name="/Atenea/AteneaGroup/Atenea.Worker/Microsoft.ServiceBus.SharedAccessKey" />
          </setting>
        </map>
        <map name="MapAtenea.Worker:Microsoft.ServiceBus.SharedAccessKeyName" kind="Identity">
          <setting>
            <aCSMoniker name="/Atenea/AteneaGroup/Atenea.Worker/Microsoft.ServiceBus.SharedAccessKeyName" />
          </setting>
        </map>
        <map name="MapAtenea.Worker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Atenea/AteneaGroup/Atenea.Worker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapAtenea.WorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Atenea/AteneaGroup/Atenea.WorkerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Atenea.Worker" generation="1" functional="0" release="0" software="C:\Projects\Atenea\Atenea\Atenea\csx\Debug\roles\Atenea.Worker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="AlteaTcp" protocol="tcp" portRanges="25832" />
            </componentports>
            <settings>
              <aCS name="Microsoft.ServiceBus.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.QueueName" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.ServiceNamespace" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.SharedAccessKey" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.SharedAccessKeyName" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Atenea.Worker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Atenea.Worker&quot;&gt;&lt;e name=&quot;AlteaTcp&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Atenea/AteneaGroup/Atenea.WorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Atenea/AteneaGroup/Atenea.WorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Atenea/AteneaGroup/Atenea.WorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="Atenea.WorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="Atenea.WorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="Atenea.WorkerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="bc851e71-54ef-41e9-86fa-6b335e3c5d72" ref="Microsoft.RedDog.Contract\ServiceContract\AteneaContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="501f3c00-04ff-43b4-9641-f27099a600b5" ref="Microsoft.RedDog.Contract\Interface\Atenea.Worker:AlteaTcp@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Atenea/AteneaGroup/Atenea.Worker:AlteaTcp" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>