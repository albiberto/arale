namespace SlackAlertOwner.Notifier.Services
{
    using Model;

    public static class MessageService
    {
        public static string Today(TeamMate teamMate)
        {
            return @$"{{
                ""blocks"": [
                    {{
                        ""type"": ""section"",
                        ""text"": {{
                            ""type"": ""plain_text"",
                            ""text"": ""Ciao <@{teamMate.Id}>. Today is your shift!"",
                            ""emoji"": true
                        }}
                    }},
                    {{
                        ""type"": ""image"",
                        ""title"": {{
                            ""type"":""plain_text"",
                            ""text"": ""image1"",
                            ""emoji"": true
                        }},
                        ""image_url"": ""https://www.testedich.de/quiz33/picture/pic_1399473109_1.jpg"",
                        ""alt_text"": ""image1""
                    }}
                ]
            }}";
        }

        public static string Tomorrow(TeamMate teamMate)
        {
            return @$"{{
                ""blocks"": [
                    {{
                        ""type"": ""section"",
                        ""text"": {{
                            ""type"": ""plain_text"",
                            ""text"": ""Ciao <@{teamMate.Id}>. Tomorrow will be your shift!"",
                            ""emoji"": true
                        }}
                    }},
                    {{
                        ""type"": ""image"",
                        ""title"": {{
                            ""type"":""plain_text"",
                            ""text"": ""image1"",
                            ""emoji"": true
                        }},
                        ""image_url"": ""https://i.pinimg.com/originals/f2/ae/90/f2ae903a3ef9aa26090b84c3fa44f596.png"",
                        ""alt_text"": ""image1""
                    }}
                ]
            }}";
        }
    }
}