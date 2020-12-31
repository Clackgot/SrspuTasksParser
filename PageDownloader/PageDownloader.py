import requests
import os
import sys
import argparse
from bs4 import BeautifulSoup
from fake_useragent import UserAgent

def fileNameFixer(filename):
    filename = filename.replace('.', '')
    filename = filename.replace(':', '')
    filename = filename.replace('\\', '')
    filename = filename.replace('\?', '')
    filename+= '.html'
    return filename
def Connect(username = 'clackgot@gmail.com', password = 'uVJ3e3Uf', url = 'https://sdo.srspu.ru/login/index.php'):
    """Устанавиливаем соединение с платформой"""
    session = requests.Session()
    userAgent = UserAgent().chrome
    r = session.get(url, headers = {
        'User-Agent': userAgent
    })
    print("Первое подключение к платформе с фейковым юзерагентом:")
    print(userAgent + "\n")
    session.headers.update({'Referer':url})
    session.headers.update({'User-Agent':UserAgent().chrome})
    moodleSession = session.cookies.get('MoodleSession')
    print("MoodleSession: " + moodleSession)
    
    soup = BeautifulSoup(r.content,'html.parser')
    logintokenTag = soup.find('input', attrs = {'name':'logintoken'})
    logintoken = logintokenTag.attrs['value']
    
    postRequest = session.post(url, {
        'anchor':	"",
        'username': username,
        'password': password,
        'logintoken': logintoken
    })
    if(postRequest.status_code == 200):
        print("Успешный вход с данными:")
        print("Логин: " + username)
        print("Пароль: " + password)
    return session
def savePage(session, url):
    "Сохраняет страницу в корневую директорию"
    request = session.get(url)
    soup = BeautifulSoup(request.content,'html.parser')
    with open("../"+fileNameFixer(soup.title.text),"w",encoding="utf-8") as f:
        f.write(request.text)
def parserInit():
    """Парсер аргументов командной строки"""
    parser = argparse.ArgumentParser()
    parser.add_argument("--login", default="clackgot@gmail.com", help = "Почта аккаунта sdo.srspu.ru")
    parser.add_argument("--password", default="uVJ3e3Uf", help = "Пароль от аккаунта sdo.srspu.ru")
    parser.add_argument("--url", 
                        default="https://sdo.srspu.ru/mod/quiz/review.php?attempt=759489&cmid=194870", 
                        help = "Ссылка на страницу, которую необходимо сохранить")
    args = parser.parse_args()
    return args

def main():
    args = parserInit()
    session = Connect(args.login, args.password)
    savePage(session, args.url) 

if __name__== "__main__":
  main()
