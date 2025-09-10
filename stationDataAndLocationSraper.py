from bs4 import BeautifulSoup
import requests
import pandas as pd
from datetime import datetime
import logging



class ScrapeStationData:

    def __init__(self):
        pass

    def get_station_details(self):

        stations = []


        for i in range(1, 27):

            url = f'https://www.doogal.co.uk/UkStations?page={i}'

            print(url)

            r = requests.get(url)
            soup = BeautifulSoup(r.text, 'html.parser')

            table = soup.find('table', class_='stationsTable table table-striped table-hover')

            if table != None:
                data = table.find('tbody')

                if data != None:
                    for row in data.find_all('tr'):
                        cols = row.find_all('td')

                        station_name = cols[0]
                        code = cols[1].find(string=True, recursive=False)
                        owner = cols[2]

                        a_tag = station_name.find('a')

                        name = a_tag.find(string=True, recursive=False).strip()
                        station_url = a_tag['href']

                        additionalDataUrl = f'https://www.doogal.co.uk/{station_url}'

                        print(additionalDataUrl)

                        station_page_url = requests.get(additionalDataUrl)
                        station_page = BeautifulSoup(station_page_url.text, 'html.parser')

                        geoTable = station_page.find('div', id='collapse6')

                        if geoTable != None:
                            geoData = geoTable.find('table')

                            if geoData != None:
                                rows = geoData.find_all('tr')
                                
                                latitiude = rows[0].find_all('td')[1]
                                longitude = rows[1].find_all('td')[1]

                                if code != None:
                                    station = {
                                        'name': name,
                                        'crs': code,
                                        'owner': owner.find(string=True, recursive=True),
                                        'latitiude': latitiude.find(string=True, recursive=False).strip(),
                                        'longitude': longitude.find(string=True, recursive=False).strip(),
                                    }  

                                    print(station)

                                    stations.append(station)


        return stations


    def save_to_json(self, stations):
        stationsdf = pd.DataFrame(stations)
        stationsdf.to_json('national-rail-stations.json', orient='records')
        print(stationsdf)


if __name__ == "__main__":
    logging.basicConfig(filename='webscraper.log', encoding='utf-8', level=logging.DEBUG)
    logging.info("****************************************************")
    logging.info(str(datetime.now()) + ' - stationDataScraper.py script started')

    webscrapperBot = ScrapeStationData()

    stationsList = webscrapperBot.get_station_details()


    webscrapperBot.save_to_json(stationsList)

