using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{

    public class Rootobject
    {
        public string schema { get; set; }
        public Definitions definitions { get; set; }
        public string[] required { get; set; }
        public string type { get; set; }
        public Properties9 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Definitions
    {
        public Say say { get; set; }
        public Show show { get; set; }
        public Listen listen { get; set; }
        public Fetch_Actions fetch_actions { get; set; }
        public Name name { get; set; }
        public Redirect redirect { get; set; }
        public Messages messages { get; set; }
        public Max_Attempts max_attempts { get; set; }
        public Handoff handoff { get; set; }
    }

    public class Say
    {
        public Anyof[] anyOf { get; set; }
    }

    public class Anyof
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties
    {
        public Speech speech { get; set; }
    }

    public class Speech
    {
        public string type { get; set; }
    }

    public class Show
    {
        public string[] required { get; set; }
        public string type { get; set; }
        public Properties1 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties1
    {
        public Body body { get; set; }
        public Images images { get; set; }
        public Subject subject { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
    }

    public class Images
    {
        public string[] required { get; set; }
        public string type { get; set; }
        public Items items { get; set; }
    }

    public class Items
    {
        public string type { get; set; }
        public Properties2 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties2
    {
        public Url url { get; set; }
        public Label label { get; set; }
    }

    public class Url
    {
        public string type { get; set; }
    }

    public class Label
    {
        public string type { get; set; }
    }

    public class Subject
    {
        public string type { get; set; }
    }

    public class Listen
    {
        public string[] type { get; set; }
        public Properties3 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties3
    {
        public Tasks tasks { get; set; }
    }

    public class Tasks
    {
        public string type { get; set; }
        public Items1 items { get; set; }
    }

    public class Items1
    {
        public string type { get; set; }
    }

    public class Fetch_Actions
    {
        public string type { get; set; }
        public Properties4 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties4
    {
        public Uri uri { get; set; }
    }

    public class Uri
    {
        public string type { get; set; }
    }

    public class Name
    {
        public string type { get; set; }
    }

    public class Redirect
    {
        public Anyof1[] anyOf { get; set; }
    }

    public class Anyof1
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties5 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties5
    {
        public Uri1 uri { get; set; }
        public Method method { get; set; }
    }

    public class Uri1
    {
        public string type { get; set; }
    }

    public class Method
    {
        public string type { get; set; }
    }

    public class Messages
    {
        public string type { get; set; }
        public int minItems { get; set; }
        public Items2 items { get; set; }
    }

    public class Items2
    {
        public string type { get; set; }
        public Properties6 properties { get; set; }
        public bool additionalProperties { get; set; }
        public Anyof2[] anyOf { get; set; }
    }

    public class Properties6
    {
        public Say1 say { get; set; }
        public Show1 show { get; set; }
    }

    public class Say1
    {
        public string _ref { get; set; }
    }

    public class Show1
    {
        public string _ref { get; set; }
    }

    public class Anyof2
    {
        public string[] required { get; set; }
    }

    public class Max_Attempts
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties7 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties7
    {
        public Num_Attempts num_attempts { get; set; }
        public Redirect1 redirect { get; set; }
    }

    public class Num_Attempts
    {
        public string type { get; set; }
        public int _default { get; set; }
    }

    public class Redirect1
    {
        public string _ref { get; set; }
    }

    public class Handoff
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties8 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties8
    {
        public Channel channel { get; set; }
        public Uri2 uri { get; set; }
        public Method1 method { get; set; }
        public Params _params { get; set; }
        public Wait_Url wait_url { get; set; }
        public Wait_Url_Method wait_url_method { get; set; }
        public Action action { get; set; }
        public Action_Method action_method { get; set; }
        public Priority priority { get; set; }
        public Timeout timeout { get; set; }
        public Voice_Status_Callback_Url voice_status_callback_url { get; set; }
        public Voice_Status_Callback_Method voice_status_callback_method { get; set; }
    }

    public class Channel
    {
        public string type { get; set; }
    }

    public class Uri2
    {
        public string type { get; set; }
    }

    public class Method1
    {
        public string type { get; set; }
    }

    public class Params
    {
        public string type { get; set; }
        public Items3 items { get; set; }
    }

    public class Items3
    {
        public string type { get; set; }
    }

    public class Wait_Url
    {
        public string type { get; set; }
    }

    public class Wait_Url_Method
    {
        public string type { get; set; }
    }

    public class Action
    {
        public string type { get; set; }
    }

    public class Action_Method
    {
        public string type { get; set; }
    }

    public class Priority
    {
        public string type { get; set; }
    }

    public class Timeout
    {
        public string type { get; set; }
    }

    public class Voice_Status_Callback_Url
    {
        public string type { get; set; }
    }

    public class Voice_Status_Callback_Method
    {
        public string type { get; set; }
    }

    public class Properties9
    {
        public Actions actions { get; set; }
    }

    public class Actions
    {
        public string type { get; set; }
        public Items4 items { get; set; }
    }

    public class Items4
    {
        public string type { get; set; }
        public Properties10 properties { get; set; }
        public bool additionalProperties { get; set; }
        public Oneof[] oneOf { get; set; }
    }

    public class Properties10
    {
        public Fetch_Actions1 fetch_actions { get; set; }
        public Say2 say { get; set; }
        public Show2 show { get; set; }
        public Listen1 listen { get; set; }
        public Redirect2 redirect { get; set; }
        public Handoff1 handoff { get; set; }
        public Remember remember { get; set; }
        public Collect collect { get; set; }
    }

    public class Fetch_Actions1
    {
        public string _ref { get; set; }
    }

    public class Say2
    {
        public string _ref { get; set; }
    }

    public class Show2
    {
        public string _ref { get; set; }
    }

    public class Listen1
    {
        public string _ref { get; set; }
    }

    public class Redirect2
    {
        public string _ref { get; set; }
    }

    public class Handoff1
    {
        public string _ref { get; set; }
    }

    public class Remember
    {
        public string type { get; set; }
    }

    public class Collect
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties11 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties11
    {
        public Name1 name { get; set; }
        public Questions questions { get; set; }
        public On_Complete on_complete { get; set; }
    }

    public class Name1
    {
        public string _ref { get; set; }
    }

    public class Questions
    {
        public string type { get; set; }
        public int minItems { get; set; }
        public Items5 items { get; set; }
    }

    public class Items5
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties12 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties12
    {
        public Question question { get; set; }
        public Name2 name { get; set; }
        public Type type { get; set; }
        public Validate validate { get; set; }
        public Confirm confirm { get; set; }
        public Require require { get; set; }
    }

    public class Question
    {
        public Anyof3[] anyOf { get; set; }
    }

    public class Anyof3
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties13 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties13
    {
        public Say3 say { get; set; }
    }

    public class Say3
    {
        public string _ref { get; set; }
    }

    public class Name2
    {
        public string _ref { get; set; }
    }

    public class Type
    {
        public Anyof4[] anyOf { get; set; }
    }

    public class Anyof4
    {
        public string type { get; set; }
    }

    public class Validate
    {
        public Anyof5[] anyOf { get; set; }
    }

    public class Anyof5
    {
        public string type { get; set; }
        public int minProperties { get; set; }
        public Properties14 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties14
    {
        public Allowed_Values allowed_values { get; set; }
        public Webhook webhook { get; set; }
        public On_Success on_success { get; set; }
        public On_Failure on_failure { get; set; }
        public Max_Attempts1 max_attempts { get; set; }
    }

    public class Allowed_Values
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties15 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties15
    {
        public List list { get; set; }
    }

    public class List
    {
        public string type { get; set; }
        public int minItems { get; set; }
        public Items6 items { get; set; }
    }

    public class Items6
    {
        public string type { get; set; }
    }

    public class Webhook
    {
        public Anyof6[] anyOf { get; set; }
    }

    public class Anyof6
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties16 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties16
    {
        public Url1 url { get; set; }
        public Method2 method { get; set; }
    }

    public class Url1
    {
        public string type { get; set; }
    }

    public class Method2
    {
        public string type { get; set; }
    }

    public class On_Success
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties17 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties17
    {
        public Say4 say { get; set; }
    }

    public class Say4
    {
        public string _ref { get; set; }
    }

    public class On_Failure
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties18 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties18
    {
        public Repeat_Question repeat_question { get; set; }
        public Messages1 messages { get; set; }
    }

    public class Repeat_Question
    {
        public string type { get; set; }
        public bool _default { get; set; }
    }

    public class Messages1
    {
        public string _ref { get; set; }
    }

    public class Max_Attempts1
    {
        public string _ref { get; set; }
    }

    public class Confirm
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties19 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties19
    {
        public Say5 say { get; set; }
        public On_Confirm on_confirm { get; set; }
        public On_Reject on_reject { get; set; }
        public Max_Attempts2 max_attempts { get; set; }
    }

    public class Say5
    {
        public string _ref { get; set; }
    }

    public class On_Confirm
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties20 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties20
    {
        public Say6 say { get; set; }
    }

    public class Say6
    {
        public string _ref { get; set; }
    }

    public class On_Reject
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties21 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties21
    {
        public Messages2 messages { get; set; }
    }

    public class Messages2
    {
        public string _ref { get; set; }
    }

    public class Max_Attempts2
    {
        public string _ref { get; set; }
    }

    public class Require
    {
        public string type { get; set; }
        public Properties22 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties22
    {
        public Messages3 messages { get; set; }
        public Max_Attempts3 max_attempts { get; set; }
    }

    public class Messages3
    {
        public string _ref { get; set; }
    }

    public class Max_Attempts3
    {
        public string _ref { get; set; }
    }

    public class On_Complete
    {
        public string type { get; set; }
        public string[] required { get; set; }
        public Properties23 properties { get; set; }
        public bool additionalProperties { get; set; }
    }

    public class Properties23
    {
        public Redirect3 redirect { get; set; }
    }

    public class Redirect3
    {
        public string _ref { get; set; }
    }

    public class Oneof
    {
        public string[] required { get; set; }
    }

}
