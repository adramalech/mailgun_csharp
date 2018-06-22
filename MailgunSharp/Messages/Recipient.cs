using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MailgunSharp.Messages
{
  public sealed class Recipient : IRecipient
  {
    private readonly string name;
    public string Name
    {
      get
      {
        return name;
      }
    }

    private readonly string address;
    public string Address
    {
      get
      {
        return address;
      }
    }

    private readonly JObject variables;
    public JObject Variables
    {
      get
      {
        return variables;
      }
    }

    public Recipient(string address, string name = null, JObject variables = null)
    {
      //this will throw an exception if the address is not valid, is empty, or null.
      var checkAddress = new MailAddress(address);

      this.address = address;
      this.name = name;
      this.variables = variables;
    }

    public string ToFormattedNameAddress()
    {
      return (checkStringIfNullOrEmpty(this.name)) ? this.address : $"{this.name} <{this.address}>";
    }

    private bool checkStringIfNullOrEmpty(string str)
    {
      return (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));
    }
  }
}
