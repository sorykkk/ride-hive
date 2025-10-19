<!-- @todo: update links -->

<template>
  <footer class="site-footer">
    <div class="footer-content">
      <!-- Company/Brand Section -->
      <div class="footer-section">
        <div class="footer-logo">
          <img src="@/assets/logo/logo-1.png" alt="RideHive Logo" class="footer-logo-img" />
          <h3>RideHive</h3>
        </div>
        <p class="footer-description">
          Connecting riders without a car and drivers who can help. 
          Share cars, reduce emissions, build community.
        </p>
      </div>

      <!-- Quick Links Section -->
      <div class="footer-section">
        <h4>Quick Links</h4>
        <ul class="footer-links">
          <li><a href="/">Home</a></li>
          <li><a href="/about">About</a></li>
          <li><a href="#">Find Rides</a></li>
          <li><a href="#">Offer Ride</a></li>
          <li><a href="#">Contact</a></li>
        </ul>
      </div>

      <!-- Support Section -->
      <div class="footer-section">
        <h4>Support</h4>
        <ul class="footer-links">
          <li><a href="#">Help Center</a></li>
          <li><a href="#">Safety</a></li>
          <li><a href="#">Terms of Service</a></li>
          <li><a href="#">Privacy Policy</a></li>
          <li><a href="#">FAQ</a></li>
        </ul>
      </div>

      <!-- Contact Section -->
      <div class="footer-section">
        <h4>Get in Touch</h4>
        <div class="contact-info">
          <p>📧 support@ridehive.com</p>
          <p>📞 +40 694 201 337</p>
          <p>📍 Aleea Studenților 9, Timișoara, România</p>
        </div>
        <div class="social-links">
          <a href="#" aria-label="Facebook" title="Facebook">📘</a>
          <a href="#" aria-label="Twitter" title="Twitter">🐦</a>
          <a href="#" aria-label="Instagram" title="Instagram">📷</a>
          <a href="#" aria-label="LinkedIn" title="LinkedIn">💼</a>
        </div>
      </div>
    </div>

    <!-- Bottom Bar -->
    <div class="footer-bottom">
      <div class="footer-bottom-content">
        <p>&copy; {{ currentYear }} RideHive. All rights reserved.</p>
        <div class="footer-badges">
          <span class="badge">🚀 Fast</span>
          <span class="badge">👌 Easy</span>
          <span class="badge">⭐ Trusted</span>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const currentYear = computed(() => new Date().getFullYear())
</script>

<style scoped>
.site-footer {
  background: linear-gradient(135deg, #004d96 0%, #054682 100%);
  color: white;
  margin-top: auto;
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 48px 24px 32px;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 32px;
}

.footer-section h4 {
  color: #ffffff;
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 16px;
  border-bottom: 2px solid #1e86db;
  padding-bottom: 8px;
  display: inline-block;
}

.footer-logo {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.footer-logo-img {
  width: 40px;
  height: 40px;
  object-fit: contain;
}

.footer-logo h3 {
  color: #ffffff;
  font-size: 24px;
  font-weight: 700;
}

.footer-description {
  color: #e8f2ff;
  line-height: 1.6;
  margin-bottom: 16px;
}

.footer-links {
  list-style: none;
  padding: 0;
}

.footer-links li {
  margin-bottom: 8px;
}

.footer-links a {
  color: #e8f2ff;
  text-decoration: none;
  transition: color 0.3s ease;
}

.footer-links a:hover {
  color: #ffffff;
  text-decoration: underline;
}

.contact-info {
  margin-bottom: 16px;
}

.contact-info p {
  color: #e8f2ff;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.social-links {
  display: flex;
  gap: 12px;
}

.social-links a {
  display: inline-block;
  font-size: 24px;
  text-decoration: none;
  transition: transform 0.3s ease;
}

.social-links a:hover {
  transform: scale(1.2);
}

.footer-bottom {
  background-color: rgba(0, 0, 0, 0.2);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.footer-bottom-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.footer-bottom-content p {
  color: #e8f2ff;
  margin: 0;
}

.footer-badges {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.badge {
  background-color: rgba(255, 255, 255, 0.1);
  color: #ffffff;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

/* Responsive Design */
@media (max-width: 768px) {
  .footer-content {
    grid-template-columns: 1fr;
    gap: 24px;
    padding: 32px 16px 24px;
  }
  
  .footer-bottom-content {
    flex-direction: column;
    text-align: center;
    padding: 16px;
  }
  
  .footer-badges {
    justify-content: center;
  }
  
  .social-links {
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .footer-logo {
    justify-content: center;
    text-align: center;
  }
  
  .footer-section {
    text-align: center;
  }
  
  .footer-links {
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  .contact-info p {
    justify-content: center;
  }
}
</style>