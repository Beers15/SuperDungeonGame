from flask import Flask
from flask import request
from waitress import serve
import mysql.connector

mydb = mysql.connector.connect(
    host="localhost",
    user="root",
    passwd="moifantom",
    database="players"
)

app = Flask(__name__)


@app.route("/api_1", methods=["POST"])
def api_1():
    username = request.values.get('username')
    amount = request.values.get('amount')
    print("test " + str(request.values.get('username')))

    print(mydb)
    mycursor = mydb.cursor()

    sql = "INSERT INTO goldHolders (amount, name) VALUES(%s, %s) ON DUPLICATE KEY UPDATE amount=amount+" + amount
    val = (str(amount), username)
    mycursor.execute(sql, val)

    mydb.commit()

    print(mycursor.rowcount, "record inserted.")

    return str(request.values.get('username'))

if __name__ == "__main__":
    from waitress import serve
    serve(app, host="0.0.0.0", port=5000)
