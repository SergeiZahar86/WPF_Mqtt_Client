using System;
using System.Text;
using System.Windows;
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
        protected override void OnClosed(EventArgs e) // этот код запускается при закрытии главного окна (конец приложения)
        {
            client.Disconnect();
            base.OnClosed(e);
            App.Current.Shutdown();
        }
        private void btnSubscribe_Click(object sender, RoutedEventArgs e) // этот код запускается при нажатии кнопки «Подписаться»
        {
            string Topic = "hello";
            client.Subscribe(new string[] { Topic }, new byte[] { 2 });   
            txtReceived.Text = "";
            /*
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
            */
        }
        private void btnPublish_Click(object sender, RoutedEventArgs e) // этот код запускается при нажатии кнопки "Опубликовать"
        {
            string Topic = "hello";
            client.Publish(Topic, Encoding.UTF8.GetBytes(txtPublish.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            /*
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
            */
        }
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) // этот код запускается при получении сообщения
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(delegate {txtReceived.Text = ReceivedMessage;}); // нам нужна эта конструкция, потому что код приема
                                                                               //в библиотеке и пользовательский интерфейс с текстовым полем выполняются в разных потоках
        }
    }
}
    
