new Vue({
    el: '#app',
    data: {
        currentStep: 1,
        formData: {
            personalInfo: {
                firstName: '',
                lastName: '',
                email: '',
                phone: ''
            },
            addressInfo: {
                address: '',
                city: '',
                state: '',
                zip: ''
            },
            accountInfo: {
                username: '',
                password: '',
                confirmPassword: ''
            }
        }
    },
    methods: {
        nextStep() {
            this.currentStep++;
        },
        prevStep() {
            this.currentStep--;
        },
        submitForm() {
            axios.post('/Home/Register', this.formData)
                .then(response => {
                    alert('Registration successful!');
                })
                .catch(error => {
                    console.error('There was an error!', error);
                });
        }
    }
});
