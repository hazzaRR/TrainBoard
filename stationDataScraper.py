from bs4 import BeautifulSoup
import requests
import pandas as pd
from datetime import datetime
import logging
from string import ascii_lowercase



class ScrapeStationData:

    def __init__(self):
        pass

    def get_station_details(self):

        stations = []


        for character in ascii_lowercase:



            url = f'http://www.railwaycodes.org.uk/stations/station{character}.shtm'

            print(url)

            r = requests.get(url)
            soup = BeautifulSoup(r.text, 'html.parser')

            table = soup.find('table', id='tablesort')

            if table != None:
                data = table.find('tbody')

                if data != None:
                    for i in data.find_all('tr'):
                        cols = i.find_all('td')

                        station_name_and_code = cols[0]
                        code = station_name_and_code.find('span').find(text=True, recursive=False)

                        name = station_name_and_code.find(text=True, recursive=False)

                        if code != None:
                            station = {
                                'name': name,
                                'crs': code.replace('(', '').replace(')', '')
                            }  

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

