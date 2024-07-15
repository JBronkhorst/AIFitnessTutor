function renderBmiChart(ctx, dates, bmiScores, weightValues, waistValues) {
    var bmiChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: dates,
            datasets: [
                {
                    label: 'BMI Score',
                    data: bmiScores,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Weight (kg)',
                    data: weightValues,
                    borderColor: 'rgba(192, 75, 75, 1)',
                    borderWidth: 2,
                    fill: false
                },
                {
                    label: 'Waist Circumference (cm)',
                    data: waistValues,
                    borderColor: 'rgba(75, 75, 192, 1)',
                    borderWidth: 2,
                    fill: false
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    title: {
                        display: true,
                    }
                },
                y: {
                    title: {
                        display: true,
                    },
                    beginAtZero: true
                }
            }
        }
    });
}
