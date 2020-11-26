using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WPF_Mqtt_Client
{
    public partial class MainWindow : Window
    {
        MqttClient client;
        string clientId;
        public MainWindow()
        {
            InitializeComponent();

            //string BrokerAddress = "broker.hivemq.com"; // https://www.hivemq.com/public-mqtt-broker/  удалённый сервер
            //client = new MqttClient(BrokerAddress);

            client = new MqttClient("10.90.101.1", 1883, false, null, null, MqttSslProtocols.None); // подключение к серверу ИндасХолдинг


            // зарегистрируйте callback-функцию (мы должны реализовать, см. ниже), которая вызывается библиотекой при получении сообщения
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; // этот код запускается при получении сообщения

            // использовать уникальный идентификатор в качестве идентификатора клиента каждый раз, когда мы запускаем приложение
            //clientId = Guid.NewGuid().ToString();
            //client.Connect(clientId);

            client.Connect("sergei", "admin", "admin"); // подключение к серверу ИндасХолдинг
        }
        // этот код запускается при закрытии главного окна (конец приложения)
        protected override void OnClosed(EventArgs e)
        {
            client.Disconnect();
            base.OnClosed(e);
            App.Current.Shutdown();
        }
        // этот код запускается при нажатии кнопки «Подписаться»
        private void btnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            if (txtTopicSubscribe.Text != "")
            {
                // вся тема
                string Topic = "/sergei/" + txtTopicSubscribe.Text + "/test";

                // подписаться на тему с QoS 2
                client.Subscribe(new string[] { Topic }, new byte[] { 2 });   // нам нужны массивы
                // в качестве параметров, потому что мы можем подписаться на разные темы одним вызовом
                txtReceived.Text = "";
            }
            else
            {
                System.Windows.MessageBox.Show("Вы должны ввести тему, чтобы подписаться!");
            }
        }
        // этот код запускается при нажатии кнопки "Опубликовать"
        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (txtTopicPublish.Text != "")
            {
                // вся тема
                string Topic = "/sergei/" + txtTopicPublish.Text + "/test";

                // опубликовать сообщение с QoS 2
                client.Publish(Topic, Encoding.UTF8.GetBytes(txtPublish.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }
            else
            {
                System.Windows.MessageBox.Show("Вы должны ввести тему для публикации!");
            }
        }
        // этот код запускается при получении сообщения
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(delegate {              // нам нужна эта конструкция, потому что код приема
                //в библиотеке и пользовательский интерфейс с текстовым полем выполняются в разных потоках
                txtReceived.Text = ReceivedMessage;
            });
        }




    }
}
    
