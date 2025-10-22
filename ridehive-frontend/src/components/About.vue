<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Helper: generate a random number between min and max (inclusive)
function randomBetween(min: number, max: number, decimals = 0) {
  const rand = Math.random() * (max - min) + min
  return parseFloat(rand.toFixed(decimals))
}

// Reactive stats object
const stats = ref({
  totalRides: 0,
  activeUsers: 0,
  citiesCovered: 0,
})

// Generate random stats when component mounts
onMounted(() => {
  stats.value = {
    totalRides: Math.floor(randomBetween(5_000, 20_000)),
    activeUsers: Math.floor(randomBetween(2_000, 10_000)),
    citiesCovered: Math.floor(randomBetween(5, 25)),
  }
})
</script>

<template>
  <div class="about-page">
    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-content">
        <h1>About RideHive</h1>
        <p class="hero-subtitle">
          Connecting communities through more efficient car renting solutions
        </p>
      </div>
    </section>

    <!-- Mission Section -->
    <section class="content-section">
      <div class="container">
        <div class="mission-grid">
          <div class="mission-text">
            <h2>Our Mission</h2>
            <p>
              RideHive is dedicated to creating a more sustainable future by connecting 
              people who need rides with those who have space to share. We believe that 
              by working together, we can reduce traffic congestion, lower carbon emissions, 
              and build stronger communities.
            </p>
            <p>
              Our platform makes it easy and safe to find reliable ride-sharing opportunities, 
              whether you're commuting to work, heading to events, or planning longer trips.
            </p>
          </div>
          <div class="mission-image">
            <div class="placeholder-image">
              🚗 🌱 🤝
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="stats-section">
      <div class="container">
        <h2>Our Impact</h2>
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-number">{{ stats.totalRides.toLocaleString() }}</div>
            <div class="stat-label">Rented cars</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats.activeUsers.toLocaleString() }}</div>
            <div class="stat-label">Active Users</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ stats.citiesCovered }}</div>
            <div class="stat-label">Cities Covered</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="content-section">
      <div class="container">
        <h2 style="color: #004d96">Why Choose RideHive?</h2>
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">🔒</div>
            <h3>Safe & Secure</h3>
            <p>Verified users, secure payments, and comprehensive insurance coverage for peace of mind.</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">💰</div>
            <h3>Cost Effective</h3>
            <p>Share travel costs and save money while contributing to a more sustainable environment.</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">🌍</div>
            <h3>Eco-Friendly</h3>
            <p>Reduce your carbon footprint by sharing rides and helping create a greener future.</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">📱</div>
            <h3>Easy to Use</h3>
            <p>Simple interface, real-time updates, and seamless booking process.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta-section">
      <div class="container">
        <h2>Ready to Start Sharing?</h2>
        <p>Join thousands of users who are already making a difference</p>
        <div class="cta-buttons">
          <!-- This should be a router link -->
           <!-- Find a Ride button only for client
            Offer a Rent for owner -->
          <router-link to="/">
            <button class="btn-primary">Rent a Car</button>
          </router-link>
          <router-link to="/create-post">
            <button class="btn-secondary">Offer a Rent</button>
          </router-link>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
.about-page {
  min-height: calc(100vh - 64px);
}

.hero-section {
  background: linear-gradient(135deg, #004d96 0%, #1e86db 100%);
  color: white;
  padding: 80px 0;
  text-align: center;
}

.hero-content h1 {
  font-size: 3.5rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.hero-subtitle {
  font-size: 1.5rem;
  opacity: 0.9;
  max-width: 600px;
  margin: 0 auto;
}

.content-section {
  padding: 80px 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 24px;
}

.mission-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 60px;
  align-items: center;
}

.mission-text h2 {
  color: #004d96;
  font-size: 2.5rem;
  margin-bottom: 2rem;
}

.mission-text p {
  font-size: 1.1rem;
  line-height: 1.8;
  margin-bottom: 1.5rem;
  color: #555;
}

.placeholder-image {
  background: #f0f8ff;
  border: 2px dashed #1e86db;
  border-radius: 12px;
  height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 4rem;
}

.stats-section {
  background: #f8fafc;
  padding: 80px 0;
  text-align: center;
}

.stats-section h2 {
  color: #004d96;
  font-size: 2.5rem;
  margin-bottom: 3rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 40px;
  max-width: 800px;
  margin: 0 auto;
}

.stat-card {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.stat-number {
  font-size: 3rem;
  font-weight: 700;
  color: #004d96;
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 1.1rem;
  color: #666;
  font-weight: 500;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 40px;
  margin-top: 3rem;
}

.feature-card {
  text-align: center;
  padding: 2rem;
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.feature-card h3 {
  color: #004d96;
  font-size: 1.5rem;
  margin-bottom: 1rem;
}

.feature-card p {
  color: #666;
  line-height: 1.6;
}

.cta-section {
  background: linear-gradient(135deg, #004d96 0%, #1e86db 100%);
  color: white;
  padding: 80px 0;
  text-align: center;
}

.cta-section h2 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.cta-section p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
  opacity: 0.9;
}

.cta-buttons {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
}

.btn-primary, .btn-secondary {
  padding: 16px 32px;
  font-size: 1.1rem;
  font-weight: 600;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  text-decoration: none;
  display: inline-block;
}

.btn-primary {
  background: white;
  color: #004d96;
}

.btn-primary:hover {
  background: #f0f8ff;
  transform: translateY(-2px);
}

.btn-secondary {
  background: transparent;
  color: white;
  border: 2px solid white;
}

.btn-secondary:hover {
  background: white;
  color: #004d96;
  transform: translateY(-2px);
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-content h1 {
    font-size: 2.5rem;
  }
  
  .hero-subtitle {
    font-size: 1.2rem;
  }
  
  .mission-grid {
    grid-template-columns: 1fr;
    gap: 40px;
  }
  
  .placeholder-image {
    height: 200px;
    font-size: 3rem;
  }
  
  .cta-buttons {
    flex-direction: column;
    align-items: center;
  }
  
  .btn-primary, .btn-secondary {
    width: 100%;
    max-width: 300px;
  }
}
</style>