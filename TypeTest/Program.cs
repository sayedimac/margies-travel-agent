using System;
using System.Linq;
using Azure.AI.Projects;
using Azure.Identity;
using Azure.AI.Extensions.OpenAI;
using OpenAI.Responses;

// Check if ResponseItem is assignable to InputItem
var asm = typeof(ProjectResponsesClient).Assembly;
var inputItemType = asm.GetTypes().First(t => t.Name == "InputItem");
var responseItemType = typeof(ResponseItem);

Console.WriteLine("InputItem: " + inputItemType.FullName);
Console.WriteLine("ResponseItem: " + responseItemType.FullName);
Console.WriteLine("ResponseItem base: " + responseItemType.BaseType?.FullName);
Console.WriteLine("InputItem is base of ResponseItem: " + inputItemType.IsAssignableFrom(responseItemType));
Console.WriteLine("InputItem base: " + inputItemType.BaseType?.FullName);

// Check MessageResponseItem
var msgType = typeof(MessageResponseItem);
Console.WriteLine("\nMessageResponseItem: " + msgType.FullName);
Console.WriteLine("MessageResponseItem base: " + msgType.BaseType?.FullName);
Console.WriteLine("MessageResponseItem assignable to InputItem: " + inputItemType.IsAssignableFrom(msgType));

// Check CreateResponseOptions from OpenAI.Responses
var cro = typeof(CreateResponseOptions);
Console.WriteLine("\nCreateResponseOptions properties:");
foreach (var p in cro.GetProperties()) Console.WriteLine("  " + p.PropertyType.Name + " " + p.Name);

// Check ResponsesClient methods
var rsc = typeof(ResponsesClient);
Console.WriteLine("\nResponsesClient CreateResponse methods:");
foreach (var m in rsc.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(m => m.Name.Contains("CreateResponse")))
    Console.WriteLine("  " + m.Name + "(" + string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) + ")");



