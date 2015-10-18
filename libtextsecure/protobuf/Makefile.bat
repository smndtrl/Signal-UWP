setlocal
SET PATH=%PATH%;C:\Users\simon\.nuget\packages\Google.ProtocolBuffers\2.4.1.555\tools
ProtoGen -namespace="libtextsecure.websocket" -umbrella_classname="WebSocketProtos" -nest_classes=true  -output_directory="../websocket/" WebSocketResources.proto
REM ProtoGen -namespace="libtextsecure.push" -umbrella_classname="PushMessageProtos" -nest_classes=true  -output_directory="../push/" IncomingPushMessageSignal.proto 
ProtoGen -namespace="libtextsecure.push" -umbrella_classname="TextSecureProtos" -nest_classes=true  -output_directory="../push/" Textsecure.proto 
ProtoGen -namespace="libtextsecure.push" -umbrella_classname="ProvisioningProtos" -nest_classes=true -output_directory="../push/" Provisioning.proto 