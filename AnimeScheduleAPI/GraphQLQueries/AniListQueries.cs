using GraphQL;

namespace AnimeScheduleAPI.GraphQLQueries;

public static class AniListQueries
{
    public static GraphQLRequest BuildSchedulesQuery(long initialDate, long finalDate, int page)
    {
        return new GraphQLRequest
        {
            Query = """
                    query GetAiringSchedules($page: Int!, $initialDate: Int!, $finalDate: Int!) {
                          Page(page: $page) {
                              airingSchedules(airingAt_greater: $initialDate, airingAt_lesser: $finalDate) {
                                  mediaId
                                  episode
                                  airingAt
                                  media {
                                     siteUrl
                                     title {
                                          romaji
                                          english
                                     }
                                     coverImage {
                                          extraLarge
                                     }
                                     format
                                     type
                                     countryOfOrigin
                                  }
                              }
                          }
                     }
                    """,
            Variables = new
            {
                page,
                initialDate,
                finalDate
            }
        };
    }

    public static GraphQLRequest BuildAnimeInfoQuery(int id)
    {
        return new GraphQLRequest
        {
            Query = """
                    query GetAnimeInfo($id: Int!) {
                        Media(id: $id) {
                          id
                          siteUrl
                          coverImage {
                            extraLarge
                          }
                          title {
                            romaji
                            english
                          }
                          type
                          format
                          status
                          description
                          startDate {
                            year
                            month
                            day
                          }
                          endDate {
                            year
                            month
                            day
                          }
                          idMal
                          source
                          season
                          seasonYear
                          episodes
                          countryOfOrigin
                          genres
                          averageScore
                          nextAiringEpisode {
                            airingAt
                            episode
                          }
                        }
                      }
                    """,
            Variables = new
            {
                id
            }
        };
    }
}