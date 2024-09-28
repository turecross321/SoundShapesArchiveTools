import os

import requests
import re
import time
import urllib.parse
from datetime import datetime

FAILURE_ATTEMPTS = 3

outputPath = "E:\\soundshapes-archive\\"
getUrlsEndpoint = "web/timemap/json?url=http://www.soundshapesgame.com/&matchType=prefix&collapse=urlkey&output=json&fl=original,mimetype,timestamp,endtimestamp,groupcount,uniqcount&filter=!statuscode:[45]..&limit=10000&_=1727440367340"
last_request_time = 0


def get(endpoint: str, referer: str = None):
    global last_request_time

    long_ratelimit_sleep_time = 120
    attempts = 0
    while attempts < FAILURE_ATTEMPTS:  # Loop until the download is successful or
        try:
            time_since_last_request = time.time() - last_request_time
            avoid_ratelimit_sleep_time = max(11 - time_since_last_request, 0)

            print("Sleeping for", str(avoid_ratelimit_sleep_time), "seconds")
            time.sleep(avoid_ratelimit_sleep_time)
            last_request_time = time.time()

            url = "https://web.archive.org/" + endpoint
            print(datetime.now().time(), "GET", url)
            r = requests.get(url, headers={'Referer': referer})
            print(r.status_code)
            r.raise_for_status()  # Raise an error for bad responses

            return r
        except requests.exceptions.HTTPError as e:
            print("Bad status code. Skipping...", e.response.status_code)
            return None
        except:
            print("ERROR: Sleeping for", long_ratelimit_sleep_time, "seconds")
            time.sleep(long_ratelimit_sleep_time)  # Wait and retry the download
            long_ratelimit_sleep_time += 60
            attempts += 1
    print("FAILED TOO MANY TIMES. RETURNING NONE")
    return None

print("Fetching URLs")
response = get(getUrlsEndpoint).json()

already_downloaded_urls = os.listdir(outputPath)

urls_to_ignore = ["/psn/s_avatar/", "/resources/soundshapes/js/", "/resources/soundshapes/css/", ".jpg", ".png", ".jpeg", ".PNG", ".gif", "/forum.html"]

# [["original","mimetype","timestamp","endtimestamp","groupcount","uniqcount"],
layout = response[0]
original_url_Index = layout.index("original")
mime_type_index = layout.index("mimetype")
timestamp_index = layout.index("timestamp")
end_timestamp_index = layout.index("endtimestamp")
group_count_index = layout.index("groupcount")
unique_count_index = layout.index("uniqcount")

# Remove the first element and filter the rest
filtered_list = []
for entry in response[1:]:
    original_url = entry[original_url_Index]
    add = True
    for ignore in urls_to_ignore:
        if ignore in original_url:
            add = False
    file_name = re.sub(r'[<>:"/\\|?*]', '_', original_url)
    for already_downloaded in already_downloaded_urls:
        if file_name in already_downloaded:
            print("Skipping", file_name, "as it's already downloaded")
            add = False

    if add:
        filtered_list.append(entry)

print(str(len(filtered_list)), "urls found after filtration")

for i, entry in enumerate(filtered_list):
    original_url = entry[original_url_Index]
    mime_type = entry[mime_type_index]
    timestamp = entry[timestamp_index]
    end_timestamp = entry[end_timestamp_index]
    group_count = entry[group_count_index]
    unique = entry[unique_count_index]

    if unique_count_index > 1:
        # https://web.archive.org/__wb/sparkline?output=json&url=http%3A%2F%2Fsoundshapesgame.com%3A80%2Flevel%2FTUxRkxf7&collection=web
        encoded_url = urllib.parse.quote(original_url, safe='')
        referer = "https://web.archive.org/web/" + timestamp + "*/" + original_url
        versions = get("__wb/sparkline?output=json&url=" + encoded_url + "&collection=web", referer=referer).json()

        dates = []
        for year, months in versions["years"].items():
            for month_index, value in enumerate(months):
                # Check if the value is greater than 0 to include it
                if value > 0:
                    # Format the month index to two digits
                    month = f"{month_index + 1:02}"
                    # Create the date in the required format
                    dates.append(f"{year}{month}")
    else:
        dates = [timestamp]

    for j, date in enumerate(dates):
        year = int(date[:4])
        if year >= 2020:
            print("Date is 2020 or later. Skipping...")
            continue

        archive_url = "web/" + date + "/" + original_url
        print("Downloading", archive_url, str(i + 1) + "/" + str(len(filtered_list)),
              "[" + str(j + 1) + "/" + str(len(dates)) + "]")
        response = get(archive_url)
        if response is None:
            continue

        # Replace all invalid filename characters with underscores
        valid_filename = re.sub(r'[<>:"/\\|?*]', '_', original_url + "_" + date)

        filePath = outputPath + valid_filename

        with open(filePath, "wb") as f:
            print("Saved to", filePath)
            f.write(response.content)
